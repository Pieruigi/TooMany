using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

namespace TMOT
{
    public class GameMode2 : GameMode
    {

 

        [SerializeField]
        DiamondSpawner diamondSpawnerPrefab;

        [SerializeField]
        MonsterSpawner monsterSpawnerPrefab;

        [SerializeField]
        TimeUpSpawner timeUpSpawnerPrefab;

        [SerializeField]
        GameObject medicalSpawnerPrefab;

        [SerializeField]
        GameObject pillSpawnerPrefab;


        //[SerializeField]
        int goalCount = 20;


        //[SerializeField]
        int stepCount = 4;

        int goalProgress = 0;


        int monsterInitialSpawnCount = 12;
        int monsterRegularSpawnCount = 4;

        int monsterRegularSpawnTime = 20;

        float hunterTimeDefault = 20;
        float hunterTimeExtra = 0;

        float hunterElapsed = 0;

        bool isHunterMode = false;



        protected override void Awake()
        {
            base.Awake();

            // Spawn diamond spawner
            Instantiate(diamondSpawnerPrefab);
            Instantiate(monsterSpawnerPrefab);
            MonsterSpawner.Instance.SpawnAmount = monsterRegularSpawnCount;
            MonsterSpawner.Instance.SpawnTime = monsterRegularSpawnTime;
            Instantiate(timeUpSpawnerPrefab);
            Instantiate(medicalSpawnerPrefab);
            Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);
        }

        void Start()
        {
            OnProgressUpdated?.Invoke(goalProgress, goalCount);
        }

        void Update()
        {
            if (isHunterMode)
            {
                hunterElapsed += Time.deltaTime;

                if (hunterElapsed > hunterTimeDefault + hunterTimeExtra)
                {
                    PlayerController.Instance.SetState(PlayerState.Prey);
                }
            }
            else
            {
                if (goalCount == goalProgress)
                {
                    MonsterSpawner.Instance.StopSpawner();
                    GameManager.Instance.ReportPlayerIsWinner();
                }
                    
            }
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

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    SpawnDiamonds().Forget();
                    MonsterSpawner.Instance.StartSpawner();   
                    if(!IsLastStep()) TimeUpSpawner.Instance.StartSpawner().Forget();
                    hunterTimeExtra = 0;
                    hunterElapsed = 0;
                    isHunterMode = false;
                    break;
                case PlayerState.Hunter:
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpSpawner.Instance.StopSpawner();
                    hunterElapsed = 0;
                    isHunterMode = true;
                    break;
            }
        }

        protected override void StartGameMode()
        {
            PlayerController.Instance.SetState(PlayerState.Prey);

            MonsterSpawner.Instance.SpawnRandomMonsters(monsterInitialSpawnCount);
            //MonsterSpawner.Instance.StartSpawner();
        }

        async UniTaskVoid SpawnDiamonds()
        {

            for (int i = 0; i < stepCount; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(.25f));

                DiamondSpawner.Instance.SpawnDiamond();
            }
        }

        async UniTaskVoid CheckStepCount()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            if (goalProgress < goalCount && goalProgress % stepCount == 0)
                PlayerController.Instance.SetState(PlayerState.Hunter);
        }

     

        public bool IsLastStep()
        {
            return goalProgress >= goalCount - stepCount;
        }

        public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.Diamond:
                    // Do action
                    goalProgress++;
                    // Unspawn diamond
                    DiamondSpawner.Instance.UnspawnDiamond(customDrone.gameObject);

                    CheckStepCount().Forget();

                    OnProgressUpdated?.Invoke(goalProgress, goalCount);

                    break;

                case CustomDroneType.TimeUp:
                    hunterTimeExtra += 5f;
                    TimeUpSpawner.Instance.ReportTimeUpPicked();
                    break;
                case CustomDroneType.Medical:
                    PlayerController.Instance.Heal();
                    MedicalSpawner.Instance.ReportMedicalPicked();
                    break;
                case CustomDroneType.Pill:
                    SpeedPowerUp.Instance.BuffSpeed();
                    PillSpawner.Instance.ReportPicked();
                    break;    
            }
        }




        public float GetNextHunterTime()
        {
            if (IsLastStep()) return 0;
            return hunterTimeDefault + hunterTimeExtra;
        }

        public float GetHunterTimeRemaining()
        {
            float ret = hunterTimeDefault + hunterTimeExtra - hunterElapsed;
            if (ret < 0) ret = 0;
            return ret;


        }
        

    }
}