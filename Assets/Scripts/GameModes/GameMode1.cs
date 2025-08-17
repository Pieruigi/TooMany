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


        [SerializeField]
        GameObject timeUpSpawnerPrefab;

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        float playerChasedTime = 7;// 50;

        float playerChasingTime = 15;

        int goalTarget = 11;

        int goalStep = 0;

        float time;

        float elapsed = 0;

        bool playerChasing = false;

        float extraChasingTime = 0;

        float monsterSpawnTime = 15;

        int initialSpawnAmount = 10;

        int normalSpawnAmount = 4;


        protected override void Awake()
        {
            base.Awake();

            // Instantiate the time up spawner
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);



        }

        // Start is called before the first frame update
        void Start()
        {


            MonsterSpawner.Instance.SpawnRandomMonsters(initialSpawnAmount);
            MonsterSpawner.Instance.SpawnAmount = normalSpawnAmount;
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
                        GameManager.Instance.ReportPlayerIsWinner();
                }
            }


        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MonsterSpawner.OnSpawnCompleted += HandleOnMonsterSpawnCompleted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MonsterSpawner.OnSpawnCompleted -= HandleOnMonsterSpawnCompleted;
        }

        private void HandleOnMonsterSpawnCompleted(int amount)
        {
            if (amount == initialSpawnAmount)
                MonsterSpawner.Instance.SpawnAmount = normalSpawnAmount;
        }

        protected override void StartGameMode()
        {
            Init();
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

    }
}