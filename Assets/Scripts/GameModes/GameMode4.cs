using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT
{
    public class GameMode4 : GameMode
    {
        public delegate void HunterTimeIncreasedDelegate(float time);
        public static HunterTimeIncreasedDelegate OnHunterTimeIncreased;


        [SerializeField]
        GameObject timeUpSpawnerPrefab;

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        float hunterTime = 5;
        public float HunterTime
        {
            get{ return hunterTime; }
        }

        int goal = 40;

        int progress = 0;

        float switchCooldown = 5;

        float switchCooldownTimer = 0;

        bool playing = false;

        float monsterSpawnTime = 15;

        int initialSpawnAmount = 10;

        int normalSpawnAmount = 4;

        protected override void Awake()
        {
            base.Awake();

            // Instantiate spawners
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            // Stop spawners
            MonsterSpawner.Instance.StopSpawner();
            TimeUpSpawner.Instance.StopSpawner();
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
            }

            // Check hunting time
            CheckHunterTime();

            if (Input.GetKeyDown(KeyCode.E))
                Switch();


        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    // Start spawners
                    MonsterSpawner.Instance.StartSpawner();
                    TimeUpSpawner.Instance.StartSpawner().Forget();
                    break;
                case PlayerState.Hunter:
                    // Stop spawners
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpSpawner.Instance.StopSpawner();
                    break;
            }
        }

        protected override void StartGameMode()
        {
            // Set prey state
            PlayerController.Instance.SetState(!StartInHuntingMode ? PlayerState.Prey : PlayerState.Hunter);

            // Spawn the first bunch of monsters
            MonsterSpawner.Instance.SpawnRandomMonsters(initialSpawnAmount, false);

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
                    TimeUpSpawner.Instance.ReportTimeUpPicked();
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
            }
            else if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // You can always switch from hunter to prey
                PlayerController.Instance.SetState(PlayerState.Prey);

                // Set the cooldown
                switchCooldownTimer = switchCooldown;
            }



        }

        void CheckHunterTime()
        {
            if (PlayerController.Instance.State != PlayerState.Hunter) return;

            hunterTime -= Time.deltaTime;

            if (hunterTime < 0)
            {
                hunterTime = 0;
                PlayerController.Instance.SetState(PlayerState.Prey);
            }

            
        }
        
    }

}
