using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public class GameMode4 : GameMode
    {
        public delegate void HunterTimeIncreasedDelegate(float time);
        public static HunterTimeIncreasedDelegate OnHunterTimeIncreased;

        public static UnityAction OnSwitchCooldownStarted;
        public static UnityAction OnSwitchCooldownCompleted;


        [SerializeField]
        GameObject timeUpSpawnerPrefab;

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        
        float hunterTime = 15;
        public float HunterTime
        {
            get { return hunterTime; }
        }

        int goal = 50;
        public int Goal
        {
            get { return goal; }
        }

        int progress = 0;

        float switchCooldown = 10;

        float switchBackCooldown = 0;//5;

        float switchCooldownTimer = 0;

        bool playing = false;

        /// <summary>
        ////////// Step10: 2.5 with auto timeup
        /// Step1: 4.625f no auto timeup
        /// Step10: 3.5 no auto timeup - suggested
        /// </summary>
        float monsterSpawnTime = 4.625f;//15;

        int initialSpawnAmount = 10;

        int normalSpawnAmount = 1;

        int backFromHunterAmount = 0;//6;

        bool firstSpawn = true;

        float hunterTimeUpSpeed = 0.1f;
        float hunterTimeUpElapsed = 0;

        float spawnElapsed = 0;


        protected override void Awake()
        {
            base.Awake();

            // Instantiate spawners
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            // Stop spawners
            MonsterSpawner.Instance.StopSpawner();
            TimeUpMultiSpawner.Instance.StopSpawner();

            //monsterSpawnTime /= .85f + (GameManager.Instance.GameStage * StageManager.StepMultiplier);

            float[] diffs = new float[] { 2f, 3.5f }; // 3.5
            float step = (diffs[1] - diffs[0]) / 9f;
            monsterSpawnTime = diffs[1] - step * GameManager.Instance.GameStage;
            int goalMax = 50;
            int goalMin = (int)(goalMax * diffs[0] / diffs[1]);
            goal = (int)Mathf.Lerp(goalMin, goalMax, GameManager.Instance.GameStage / 9);
            
            
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (!playing) return;

            if (switchCooldownTimer > 0)
            {
                switchCooldownTimer -= Time.deltaTime;

                 if (switchCooldownTimer <= 0)
                     OnSwitchCooldownCompleted?.Invoke();
            }

            if (progress >= goal)
            {
                if (PlayerController.Instance.State != PlayerState.Dead)
                {
                    playing = false;
                    GameManager.Instance.ReportPlayerIsWinner();

                    return;
                }

            }

            // Check hunter time up
            //CheckHunterTimeUp();

            // Check hunting time
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

        protected override void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            base.HandleOnGameStateChanged(oldState, newState);

            switch (newState)
            {
                case GameState.Winner:
                case GameState.Loser:
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpMultiSpawner.Instance.StopSpawner();
                    break;
            }
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    spawnElapsed = 0;
                    // Spawn the first bunch of monsters
                    var amount = backFromHunterAmount;
                    if (firstSpawn)
                    {
                        amount = initialSpawnAmount;
                        firstSpawn = false;
                    }
                    hunterTimeUpElapsed = 0;
                    MonsterSpawner.Instance.SpawnRandomMonsters(amount, false);
                    // Start spawners
                    MonsterSpawner.Instance.StartSpawner();
                    TimeUpMultiSpawner.Instance.StartSpawner().Forget();
                    break;
                case PlayerState.Hunter:
                    spawnElapsed = 0;
                    hunterTimeUpElapsed = 0;
                    // Stop spawners
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpMultiSpawner.Instance.StopSpawner();
                    break;

            }
        }

        protected override void StartGameMode()
        {
            // Set prey state
            PlayerController.Instance.SetState(!StartInHuntingMode ? PlayerState.Prey : PlayerState.Hunter);

            // Initialize spawner
            MonsterSpawner.Instance.SpawnAmount = normalSpawnAmount;
            MonsterSpawner.Instance.SpawnTime = monsterSpawnTime;

            playing = true;
        }

        public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.TimeUp:
                    hunterTime += 5;
                    TimeUpMultiSpawner.Instance.ReportTimeUpPicked(customDrone.gameObject);
                    OnHunterTimeIncreased?.Invoke(hunterTime);
                    break;
              
            }
        }

        public override void ReportMonsterDroneHitByPlayer(MonsterController monsterDrone)
        {
            base.ReportMonsterDroneHitByPlayer(monsterDrone);

            progress++;

            OnProgressUpdated?.Invoke(progress, goal);

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
                //OnSwitchCooldownStarted?.Invoke();
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
        
    }

}
