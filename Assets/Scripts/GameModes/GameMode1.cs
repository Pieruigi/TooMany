using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public class GameMode1 : GameMode
    {
       

        [SerializeField]
        GameObject timeUpSpawnerPrefab;

        float playerChasedTime = 90;

        float playerChasingTime = 20;

        int goalTarget = 11;//6;

        int goalStep = 0;

        float time;

        float elapsed = 0;

        bool playerChasing = false;

        float extraChasingTime = 0;



        protected override void Awake()
        {
            base.Awake();

            // Instantiate the time up spawner
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);

        }

        // Start is called before the first frame update
        void Start()
        {

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
                MonsterSpawner.Instance.StartSpawner();
                if (!IsLastStep())
                    TimeUpSpawner.Instance.StartSpawner();
             

            }

        }


        public bool IsLastStep()
        {
            return goalStep == goalTarget-1;
        }

        public float GetGoalTimeRemaining()
        {
            var total = (goalTarget+1)/2 * playerChasedTime;
            Debug.Log("TOTAL:" + total);

            var passed = (goalStep+1)/2 * playerChasedTime;
             Debug.Log("PASSED:" + passed);
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

        

    }
}