using System.Collections;
using System.Collections.Generic;
using TMOT.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public abstract class GameMode : Singleton<GameMode>
    {
        public delegate void ProgressUpdatedDelegate(int progress, int goal);
        public static ProgressUpdatedDelegate OnProgressUpdated;


        [SerializeField]
        GameObject gameUIPrefab;

        [SerializeField]
        bool startInHuntingMode = false;
        public bool StartInHuntingMode
        {
            get { return startInHuntingMode; }
        }

        [SerializeField]
        GameObject medicalSpawnerPrefab;

        [SerializeField]
        GameObject pillSpawnerPrefab;

        [SerializeField]
        GameObject batterySpawnerPrefab;

        [SerializeField]
        GameObject shieldSpawnerPrefab;

        protected abstract void StartGameMode();

        //GameObject gameUI;

        protected override void Awake()
        {
            base.Awake();

            Time.timeScale = GameManager.Instance.GameSpeed;

            Instantiate(gameUIPrefab);

            if (batterySpawnerPrefab)
                Instantiate(batterySpawnerPrefab, Vector3.zero, Quaternion.identity);

            if (medicalSpawnerPrefab)
                Instantiate(medicalSpawnerPrefab, Vector3.zero, Quaternion.identity);

            if (pillSpawnerPrefab)
                Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);

            if (shieldSpawnerPrefab)
                Instantiate(shieldSpawnerPrefab, Vector3.zero, Quaternion.identity);
        }

        protected virtual void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
        }

        protected virtual void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        protected virtual void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    StartGameMode();
                    break;

                case GameState.Winner:
                case GameState.Loser:
                    break;


            }
        }

        public virtual void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            switch (customDrone.Type)
            {
                case CustomDroneType.Battery:
                    StaminaPowerUp.Instance.BuffSpeed();
                    BatterySpawner.Instance.ReportPicked();
                    break;
                case CustomDroneType.Medical:
                    PlayerController.Instance.Heal();
                    MedicalSpawner.Instance.ReportMedicalPicked();
                    break;
                case CustomDroneType.Pill:
                    SpeedPowerUp.Instance.BuffSpeed();
                    PillSpawner.Instance.ReportPicked();
                    break;
                case CustomDroneType.Shield:
                    ShieldPowerUp.Instance.Activate();
                    ShieldSpawner.Instance.ReportPicked();
                    break;
            }
        }

        public virtual void ReportMonsterDroneHitByPlayer(MonsterController monsterDrone)
        {
            
        }

    }
}