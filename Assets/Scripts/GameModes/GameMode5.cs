using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class GameMode5 : GameMode
    {

        public delegate void HunterTimeIncreasedDelegate(float amount);
        public static HunterTimeIncreasedDelegate OnHunterTimerIncreased;

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        [SerializeField]
        MedicalSpawner medicalSpawnerPrefab;

        [SerializeField]
        TimeUpSpawner timeUpSpawnerPrefab;

        [SerializeReference]
        PillSpawner pillSpawnerPrefab;

        [SerializeField]
        DiamondSpawner diamondSpawnerPrefab;

        int initialMonsterAmount = 10;

        int monsterAmount = 4;

        float monsterSpawnTime = 10;

        float hunterTime = 10;

        float hunterTimeExtra = 0;

        float hunterTimeElapsed = 0;

        float timeUpAmount = 5;

        bool hasBomb = false;

        float explosionRange = 20;

        Vector3 bombPosition;

        protected override void Awake()
        {
            base.Awake();

            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(medicalSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(diamondSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            MonsterSpawner.Instance.StopSpawner();
            MonsterSpawner.Instance.SpawnTime = monsterSpawnTime;
            MonsterSpawner.Instance.SpawnAmount = monsterAmount;
            TimeUpSpawner.Instance.StopSpawner();


        }

        void Update()
        {
            //if (PlayerController.Instance.State != PlayerState.Prey && PlayerController.Instance.State != PlayerState.Hunter) return;

            if (PlayerController.Instance.State == PlayerState.Prey)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hasBomb)
                        ReleaseBomb();
                }

            }
            else if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                // Update hunter time
                hunterTimeElapsed += Time.deltaTime;

                if (hunterTimeElapsed > hunterTime + hunterTimeExtra)
                {
                    hunterTimeElapsed = 0;
                    SwitchToPreyMode();
                }
            }

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerController.OnStateChanged += HandleOnStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerController.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    TimeUpSpawner.Instance.StartSpawner().Forget();
                    SpawnDiamondDelayed(3).Forget();
                    MonsterSpawner.Instance.StartSpawner();

                    break;
                case PlayerState.Hunter:
                    hunterTimeElapsed = 0;
                    hunterTimeExtra = 0;
                    TimeUpSpawner.Instance.StopSpawner();
                    MonsterSpawner.Instance.StopSpawner();
                    break;
            }
        }

        protected override void StartGameMode()
        {
            // Set player
            PlayerController.Instance.SetState(PlayerState.Prey);

            // Spawn the initial amount of monsters
            MonsterSpawner.Instance.SpawnRandomMonsters(initialMonsterAmount, false);


        }




        public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.TimeUp:
                    IncreasePlayerChaseTime(timeUpAmount);
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
                case CustomDroneType.Diamond:
                    DiamondSpawner.Instance.UnspawnDiamond(customDrone.gameObject);
                    hasBomb = true;

                    break;
            }
        }

        async UniTask SpawnDiamondDelayed(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            DiamondSpawner.Instance.SpawnDiamond();
        }

        void IncreasePlayerChaseTime(float amount)
        {
            hunterTimeExtra += amount;

            OnHunterTimerIncreased?.Invoke(hunterTimeExtra + hunterTime);
        }

        void SwitchToHunterMode()
        {
            var bots = MonsterSpawner.Instance.Monsters.Where(m => Vector3.Distance(bombPosition, m.transform.position)>explosionRange).ToList();
            
            // Change behaviour for all the bots in normal behaviour not int bot list we just took
            foreach (var m in bots)
            {
                m.InvertedBehaviour = true;
            }

            PlayerController.Instance.SetState(PlayerState.Hunter);
        }

        void SwitchToPreyMode()
        {
            foreach (var m in MonsterSpawner.Instance.Monsters)
            {
                if (m.InvertedBehaviour)
                    m.InvertedBehaviour = false;
            }

            PlayerController.Instance.SetState(PlayerState.Prey);
        }

        void ReleaseBomb()
        {
            bombPosition = PlayerController.Instance.transform.position;
            SwitchToHunterMode();
        }

        
    }
}