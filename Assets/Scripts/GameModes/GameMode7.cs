using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

namespace TMOT
{
    public class GameMode7 : GameMode
    {

        [SerializeField]
        DiamondSpawner diamondSpawnerPrefab;

        [SerializeField]
        MonsterSpawner monsterSpawnerPrefab;

        [SerializeField]
        TimeUpSpawner timeUpSpawnerPrefab;

        int goalCount = 50;//20;

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

        int diamondCount = 4;

        protected override void Awake()
        {
            base.Awake();

            // Spawn diamond spawner
            Instantiate(diamondSpawnerPrefab);
            Instantiate(monsterSpawnerPrefab);
            MonsterSpawner.Instance.SpawnAmount = monsterRegularSpawnCount;
            MonsterSpawner.Instance.SpawnTime = monsterRegularSpawnTime;
            Instantiate(timeUpSpawnerPrefab);

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

            MonsterSpawner.Instance.SpawnRandomMonsters(monsterInitialSpawnCount);
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

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    SpawnDiamonds().Forget();
                    MonsterSpawner.Instance.StartSpawner();
                    hunterTimeExtra = 0;
                    hunterElapsed = 0;
                    isHunterMode = false;
                    break;
                case PlayerState.Hunter:
                    MonsterSpawner.Instance.StopSpawner();
                    TimeUpSpawner.Instance.StopSpawner();
                    DiamondSpawner.Instance.UnspawnAllDiamonds();
                    hunterElapsed = 0;
                    isHunterMode = true;
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
         

    }
}