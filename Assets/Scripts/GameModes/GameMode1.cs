using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace TMOT
{
    public class GameMode1 : GameMode
    {
        public delegate void HunterTimeIncreasedDelegate(float amount);
        public static HunterTimeIncreasedDelegate OnHunterTimeIncreased;

        [SerializeField]
        GameObject timeUpSpawnerPrefab;

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        // [SerializeField]
        // GameObject medicalSpawnerPrefab;

        // [SerializeField]
        // GameObject pillSpawnerPrefab;

  

        float playerChasedTime = 45;

        float playerChasingTime = 15;

        int goalTarget = 9;

        int goalStep = 0;

        float time;

        float elapsed = 0;

        bool playerChasing = false;

        float extraChasingTime = 0;

        /// <summary>
        /// Stage1: 4.05f    
        /// Stage 10: 2.5f
        /// </summary>

        float monsterSpawnTime = 2.5f;//15;

        int initialSpawnAmount = 10;

        int normalSpawnAmount = 1;//6;

        bool initialized = false;

        float spawnElapsed = 0;

        //float[] diffs = new float[] { 2.5f, 4.1f };


        protected override void Awake()
        {
            base.Awake();

            // Instantiate the time up spawner
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            // Instantiate(medicalSpawnerPrefab, Vector3.zero, Quaternion.identity);
            // Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);

            // Init spawn time
            // 2.5 - 4.0
            float[] diffs = new float[] { 2.5f, 4f };
            float step = (diffs[1] - diffs[0]) / 9f;
            monsterSpawnTime = diffs[1] - step * GameManager.Instance.GameStage;


            //monsterSpawnTime /= 1 + GameManager.Instance.GameStage * StageManager.StepMultiplier;
            //monsterSpawnTime = 4.05f;
        }

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            //Time.timeScale = 1.6f;
#endif

            
        }

        // Update is called once per frame
        void Update()
        {


            if (GameManager.Instance.GameState != GameState.Playing) return;

            elapsed += Time.deltaTime;


            if (elapsed > time)
            {
                elapsed = 0;

                if (!GoalReached())
                {
                    playerChasing = !playerChasing;
                    Init();
                }
                else
                {
                    if (PlayerController.Instance.State != PlayerState.Dead)
                    {
                        GameManager.Instance.ReportPlayerIsWinner();
                        MonsterSpawner.Instance.StopSpawner();
                    }

                }
            }

            CheckMonsterSpawn();
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MonsterSpawner.OnSpawnCompleted += HandleOnMonsterSpawnCompleted;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MonsterSpawner.OnSpawnCompleted -= HandleOnMonsterSpawnCompleted;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                case PlayerState.Hunter:
                    spawnElapsed = 0;
                    break;
            }
        }

        private void HandleOnMonsterSpawnCompleted(int amount)
        {
            // if (amount == initialSpawnAmount)
            //     MonsterSpawner.Instance.SpawnAmount = normalSpawnAmount;
        }

        protected override void StartGameMode()
        {
            MonsterSpawner.Instance.SpawnRandomMonsters(initialSpawnAmount);
            //MonsterSpawner.Instance.SpawnAmount = normalSpawnAmount;
            Init();
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

        bool GoalReached()
        {
            //if (!playerChasing)
            goalStep++;

            return (goalStep == goalTarget);
        }


        void Init()
        {
            elapsed = 0;
            if (!playerChasing)
            {
                extraChasingTime = 0;
                time = playerChasedTime;
            }
            else
            {
                time = playerChasingTime + extraChasingTime;
            }

            PlayerController.Instance.SetState(!playerChasing ? PlayerState.Prey : PlayerState.Hunter);
            if (playerChasing)
            {
                MonsterSpawner.Instance.StopSpawner();
                TimeUpSpawner.Instance.StopSpawner();
            }

            else
            {
                MonsterSpawner.Instance.SpawnTime = monsterSpawnTime;
                MonsterSpawner.Instance.StartSpawner();
                if (!IsLastStep())
                    TimeUpSpawner.Instance.StartSpawner().Forget();


            }

            if (playerChasing && initialized)
                OnProgressUpdated?.Invoke(GetCurrentStep() / 2 + 1, (GetStepMax() + 1) / 2);

            initialized = true;
        }


        public bool IsLastStep()
        {
            return goalStep == goalTarget - 1;
        }

        public float GetGoalTimeRemaining()
        {
            var total = (goalTarget + 1) / 2 * playerChasedTime;


            var passed = (goalStep + 1) / 2 * playerChasedTime;

            if (!playerChasing)
                passed += elapsed;


            return total - passed;


        }

        public float GetSwitchTimeLeft()
        {
            return time - elapsed;
        }

        public float GetChasingTimeLeft()
        {
            if (!playerChasing)
                return playerChasingTime + extraChasingTime;
            else
                return playerChasingTime + extraChasingTime - elapsed;
        }

        public void IncreasePlayerChaseTime(float amount)
        {
            if (playerChasing) return;
            extraChasingTime += amount;

            OnHunterTimeIncreased?.Invoke(amount);
        }

        public float GetTimeRemaining()
        {
            return Mathf.Max(0, time - elapsed);
        }

        public float GetNextPreyTime()
        {
            return playerChasedTime;
        }

        public float GetNextHunterTime()
        {
            if (IsLastStep()) return 0;
            return playerChasingTime + extraChasingTime;
        }

        public int GetCurrentStep()
        {
            return goalStep;
        }

        public int GetStepMax()
        {
            return goalTarget;
        }
        public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.TimeUp:
                    IncreasePlayerChaseTime(5f);
                    TimeUpSpawner.Instance.ReportTimeUpPicked();
                    break;
                // case CustomDroneType.Medical:
                //     PlayerController.Instance.Heal();
                //     MedicalSpawner.Instance.ReportMedicalPicked();
                //     break;    
                // case CustomDroneType.Pill:
                //     SpeedPowerUp.Instance.BuffSpeed();
                //     PillSpawner.Instance.ReportPicked();
                //     break;    
            }
        }


    }
}