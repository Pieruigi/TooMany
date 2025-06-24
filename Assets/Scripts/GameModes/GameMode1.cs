using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode1 : GameMode
    {
       
        float playerChasedTime = 90;

        float playerChasingTime = 20;

        float goalTarget = 6;

        float goalStep = 0;

        float time;

        float elapsed = 0;

        bool playerChasing = false;

               

        // Start is called before the first frame update
        void Start()
        {
            Init();
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


            }
        }

        bool GoalReached()
        {
            if (!playerChasing)
                goalStep++;

            return !(goalStep < goalTarget);
        }


        void Init()
        {
            elapsed = 0;
            if (!playerChasing)
                time = playerChasedTime;
            else
                time = playerChasingTime;


            PlayerController.Instance.SetState(!playerChasing ? PlayerState.Prey : PlayerState.Hunter);
            if (playerChasing)
                MonsterSpawner.Instance.StopSpawner();
            else
                MonsterSpawner.Instance.StartSpawner();
        }
        
        public float GetGoalTimeRemaining()
        {
            var total = goalTarget * playerChasedTime;

            var passed = goalStep * playerChasedTime;
            if (!playerChasing)
                passed += elapsed;

            return total - passed;

            
        }
    }
}