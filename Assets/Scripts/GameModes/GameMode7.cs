using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public class GameMode7 : GameMode
    {
        public delegate void HunterTimeIncreasedDelegate(float time);
        public static HunterTimeIncreasedDelegate OnHunterTimeIncreased;

        public static UnityAction OnSwitchCooldownStarted;
        public static UnityAction OnSwitchCooldownCompleted;

        [SerializeField]
        DiamondSpawner diamondSpawnerPrefab;

        [SerializeField]
        MonsterSpawner monsterSpawnerPrefab;

        [SerializeField]
        TimeUpSpawner timeUpSpawnerPrefab;

        int goalCount = 20;//20;
        public int Goal
        {
            get{ return goalCount; }
        }

        //[SerializeField]
        int stepCount = 4;

        int goalProgress = 0;

        int monsterInitialSpawnCount = 12;
        int monsterRegularSpawnCount = 3;

        //int monsterRegularSpawnTime = 3;

        float hunterTimeDefault = 20;
        float hunterTimeExtra = 0;

        float hunterElapsed = 0;

        bool isHunterMode = false;

        int diamondCount = 4;

        float hunterTime = 15;
        public float HunterTime
        {
            get{ return hunterTime; }
        }

        float switchCooldown = 10;

        float switchBackCooldown = 0;

        float switchCooldownTimer = 0;
        public float SwitchCooldownTimer
        {
            get{ return switchCooldownTimer; }
        }

        float elapsed = 0;

        float hunterTimeUpSpeed = 0.1f;
        float hunterTimeUpElapsed = 0;

        int backFromHunterAmount = 0;

        bool firstSpawn = true;

         int initialSpawnAmount = 10;
         
         /// <summary>
         /// Step1: 6.2
         /// Step10: 3.5
         /// </summary>
         float monsterSpawnTime = 6.2f;

        float spawnElapsed = 0;

        protected override void Awake()
        {
            base.Awake();

            // Spawn diamond spawner
            Instantiate(diamondSpawnerPrefab);
            Instantiate(monsterSpawnerPrefab);
            MonsterSpawner.Instance.SpawnAmount = monsterRegularSpawnCount;
            //MonsterSpawner.Instance.SpawnTime = monsterRegularSpawnTime;
            Instantiate(timeUpSpawnerPrefab);

            // 3 - 6
            float[] diffs = new float[] { 3f, 6f };
            float step = (diffs[1] - diffs[0]) / 9f;
            monsterSpawnTime = diffs[1] - step * GameManager.Instance.GameStage;
  
        }

        void Update()
        {
            if (GameManager.Instance.GameState != GameState.Playing) return;

            elapsed += Time.deltaTime;


            if (switchCooldownTimer > 0)
            {
                switchCooldownTimer -= Time.deltaTime;

                if (switchCooldownTimer <= 0)
                     OnSwitchCooldownCompleted?.Invoke();
            }

            if (goalProgress >= goalCount)
            {
                if (PlayerController.Instance.State != PlayerState.Dead)
                {
                    GameManager.Instance.ReportPlayerIsWinner();
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpSpawner.Instance.StopSpawner();
                    return;
                }

            }

            CheckHunterTimeUp();

            CheckHunterTime();

            if (Input.GetKeyDown(KeyCode.E))
                Switch();

            CheckMonsterSpawn();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
          
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
      
        }


        protected override void StartGameMode()
        {
            PlayerController.Instance.SetState(PlayerState.Prey);

            //MonsterSpawner.Instance.SpawnRandomMonsters(monsterInitialSpawnCount);
        }

         void CheckMonsterSpawn()
        {
            if (PlayerController.Instance.State != PlayerState.Prey)
                return;

            spawnElapsed += Time.deltaTime;
            if (spawnElapsed > monsterSpawnTime)
            {
                spawnElapsed -= monsterSpawnTime;
                MonsterSpawner.Instance.SpawnRandomMonsters(1);
            }
        }

        public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.Diamond:
                    // Do action
                    //goalProgress++;
                    goalProgress++;

                    // Unspawn diamond
                    DiamondSpawner.Instance.UnspawnDiamond(customDrone.gameObject);

                    SpawnDiamondDelayed(5f).Forget();

                    OnProgressUpdated?.Invoke(goalProgress, goalCount);

                    break;

                case CustomDroneType.TimeUp:
                    hunterTime += 5f;
                    TimeUpSpawner.Instance.ReportTimeUpPicked();
                    OnHunterTimeIncreased?.Invoke(hunterTime);
                    break;

            }
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    var amount = backFromHunterAmount;
                    if (firstSpawn)
                    {
                        amount = initialSpawnAmount;
                        firstSpawn = false;
                    }
                    MonsterSpawner.Instance.SpawnRandomMonsters(amount, false);
                    SpawnDiamonds().Forget();
                    MonsterSpawner.Instance.StartSpawner();
                    TimeUpSpawner.Instance.StartSpawner().Forget();
                    hunterTimeExtra = 0;
                    hunterElapsed = 0;
                    isHunterMode = false;
                    hunterTimeUpElapsed = 0;
                    spawnElapsed = 0;
                    break;
                case PlayerState.Hunter:
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpSpawner.Instance.StopSpawner();
                    DiamondSpawner.Instance.UnspawnAllDiamonds();
                    hunterElapsed = 0;
                    isHunterMode = true;
                    hunterTimeUpElapsed = 0;
                    spawnElapsed = 0;
                    break;
            }
        }


        async UniTaskVoid SpawnDiamondDelayed(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            if (PlayerController.Instance.State != PlayerState.Prey) return;

            DiamondSpawner.Instance.SpawnDiamond();
        }

        async UniTaskVoid SpawnDiamonds()
        {
            for (int i = 0; i < diamondCount; i++)
            {
                DiamondSpawner.Instance.SpawnDiamond();
                await UniTask.Delay(TimeSpan.FromSeconds(.25f));
            }
        }

        void CheckHunterTime()
        {
            if (PlayerController.Instance.State != PlayerState.Hunter) return;

            hunterTime -= Time.deltaTime;

            if (hunterTime < 0)
            {
                hunterTime = 0;
                switchCooldownTimer = switchCooldown;
                PlayerController.Instance.SetState(PlayerState.Prey);

            }


        }
        
        void CheckHunterTimeUp()
        {
            if (PlayerController.Instance.State != PlayerState.Prey) return;

            hunterTimeUpElapsed += Time.deltaTime * hunterTimeUpSpeed;

            if (hunterTimeUpElapsed > 1)
            {
                hunterTimeUpElapsed -= 1;
                hunterTime++;
                OnHunterTimeIncreased?.Invoke(hunterTime);
            }
        }

        void Switch()
        {
            if (PlayerController.Instance.State == PlayerState.Prey)
            {
                // Still in cooldown or no chase time left
                if (switchCooldownTimer > 0 || hunterTime == 0) return;

                // Switch
                PlayerController.Instance.SetState(PlayerState.Hunter);

                switchCooldownTimer = switchBackCooldown;
            }
            else if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                if (switchCooldownTimer > 0) return;

                // You can always switch from hunter to prey
                PlayerController.Instance.SetState(PlayerState.Prey);

                // Set the cooldown
                switchCooldownTimer = switchCooldown;
                OnSwitchCooldownStarted?.Invoke();
            }

        }
         

    }
}