using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class GameMode3 : GameMode
    {
        public delegate void ExtraTimeOnKillDelegate(float amount);
        public static ExtraTimeOnKillDelegate OnExtraTimeOnKillIncreased;

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

        int goal = 100;
        public int Goal
        {
            get{ return goal; }
        }

        int progress = 0;


        float hunterTime = 10;
        public float HunterTime
        {
            get{ return hunterTime; }
        }

        float hunterTimeExtra = 0;

        float hunterTimeClockAmount = 5f;

        float extraTimeOnKill = 1f;

        bool loop = false;

        float spawnTime = 10;

        int initialSpawnCount = 10;

        int normalSpawnCount = 4;
        float spawnElapsed = 0;

        float hunterTimeElapsed = 0;

        float diamondDelay = 3;

     
        



        protected override void Awake()
        {
            base.Awake();

            // Instantiate bot spawner
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            MonsterSpawner.Instance.SpawnTime = spawnTime;
            MonsterSpawner.Instance.SpawnAmount = normalSpawnCount;
            MonsterSpawner.Instance.StopSpawner();
            // Instantiate timer up spawner
            Instantiate(timeUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            TimeUpSpawner.Instance.StopSpawner();
            // Instatiate diamond spawner
            Instantiate(diamondSpawnerPrefab, Vector3.zero, Quaternion.identity);

            // Instantiate medical spawner
            Instantiate(medicalSpawnerPrefab, Vector3.zero, Quaternion.identity);
            // Instantiate pills spawner
            Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);

        }

        // Start is called before the first frame update
        void Start()
        {
            OnProgressUpdated?.Invoke(progress, goal);
        }


        // Update is called once per frame
        void Update()
        {
            if (!loop) return;

            UpdateSwitchTime();

            UpdateSpawnTime();
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
            Debug.Log("TEST - Starting game mode...");
            // Set player state
            PlayerController.Instance.SetState(PlayerState.Prey);

            // Spawn blue and red bots
            MonsterSpawner.Instance.SpawnRandomMonsters(initialSpawnCount);

            
            loop = true;
            
        }



        public override void ReportMonsterDroneHitByPlayer(MonsterController monsterDrone)
        {
            base.ReportMonsterDroneHitByPlayer(monsterDrone);

            // Switch a new drone to victim
            var newVictims = MonsterSpawner.Instance.Monsters.Where(m=>m.InvertedBehaviour).OrderBy(m => Vector3.Distance(PlayerController.Instance.transform.position, m.transform.position)).Take(1).ToList();
            Debug.Log($"TEST - newVictims.Count:{newVictims.Count}");
            foreach(var v in newVictims)
                v.ForceToPrey();


            hunterTimeExtra += extraTimeOnKill;

            OnExtraTimeOnKillIncreased?.Invoke(extraTimeOnKill);

            progress++;

            OnProgressUpdated?.Invoke(progress, goal);

            if (progress >= goal)
            {
                loop = false;
                GameManager.Instance.ReportPlayerIsWinner();
            }   
        }

         public override void ReportCustomDronePicked(CustomDroneController customDrone)
        {
            base.ReportCustomDronePicked(customDrone);

            switch (customDrone.Type)
            {
                case CustomDroneType.TimeUp:
                    IncreasePlayerChaseTime(hunterTimeClockAmount);
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
                    SwitchToHunterMode();
                    
                    break;  
            }
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    // Spawn diamond
                    SpawnDiamondDelayed(diamondDelay).Forget();
                    TimeUpSpawner.Instance.StartSpawner().Forget();
                    break;
                case PlayerState.Hunter:
                    TimeUpSpawner.Instance.StopSpawner();
                    break;
            }    
        }

        async UniTaskVoid SpawnDiamondDelayed(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            DiamondSpawner.Instance.SpawnDiamond();
        }

        void SwitchToHunterMode()
        {
            // How many bots we must transform
            int count = MonsterSpawner.Instance.Monsters.Count / 4;// + (int)(hunterTimeExtra / hunterTimeClockAmount) * MonsterSpawner.Instance.Monsters.Count / 10;

            // Clamp value 
            count = Mathf.Min(count, MonsterSpawner.Instance.Monsters.Count);

            //count = 1;
 
            // Choose bots
            var bots = MonsterSpawner.Instance.Monsters.ToList().OrderBy(x => Vector3.Distance(PlayerController.Instance.transform.position, x.transform.position)).Take(count).ToList();

            // if (bots.Count < count)
            // {
            //     // It means there are enough bots to switch to blue but most of them are away from player
            //     var diff = count - bots.Count;
            //     var diffL = MonsterSpawner.Instance.Monsters.ToList().Where(m => !bots.Contains(m)).OrderBy(x => UnityEngine.Random.value).Take(diff).ToList();
            //     bots.AddRange(diffL);
            // }

            // Change behaviour for all the bots in normal behaviour not int bot list we just took
            foreach (var m in MonsterSpawner.Instance.Monsters)
            {
                if (bots.Contains(m)) continue;
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

        void IncreasePlayerChaseTime(float amount)
        {
            hunterTimeExtra += amount;

            OnHunterTimerIncreased?.Invoke(hunterTimeExtra+hunterTime);
        }

        void UpdateSwitchTime()
        {
            if (PlayerController.Instance.State != PlayerState.Hunter) return;

            hunterTimeElapsed += Time.deltaTime;

            if (hunterTimeElapsed > hunterTime + hunterTimeExtra)
            {
                hunterTimeElapsed = 0;
                hunterTimeExtra = 0;
                spawnElapsed = 0;

                // Switch
                SwitchToPreyMode();
                
                

            }
        }

        void UpdateSpawnTime()
        {
            if (PlayerController.Instance.State != PlayerState.Prey) return;

            spawnElapsed += Time.deltaTime;

            if (spawnElapsed > spawnTime)
            {
                spawnElapsed -= spawnTime;

                MonsterSpawner.Instance.SpawnRandomMonsters(normalSpawnCount);
            }
        }

        

        public float GetTimeLeft()
        {
            if (PlayerController.Instance.State != PlayerState.Hunter) return 0;
            float ret = hunterTime + hunterTimeExtra - hunterTimeElapsed;
            if (ret < 0) ret = 0;
            return ret;
        }
    }
}