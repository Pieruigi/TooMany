using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode3 : GameMode
    {
        [SerializeField]
        GameObject monsterSpawnerPrefab;

        int goal = 60;

        int progress = 0;

        string progressStringFormat = "{0}/{1}";

        int stage = 0;

        float switchTime = 30;

        float switchElapsed = 0;

        bool loop = false;

        bool isBlue = false;

        int blueCountAtStart = 10;
        int redCountAtStart = 10;

        float spawnTime = 8;

        int spawnAmount = 3;
        float spawnElapsed = 0;



        protected override void Awake()
        {
            base.Awake();

            // Instantiate bot spawner
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            MonsterSpawner.Instance.StopSpawner();
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void StartGameMode()
        {
            Debug.Log("TEST - Starting game mode...");
            // Set player state
            PlayerController.Instance.SetState(PlayerState.Prey);

            // Spawn blue and red bots
            MonsterSpawner.Instance.SpawnRandomMonsters(blueCountAtStart);
            MonsterSpawner.Instance.SpawnRandomMonsters(blueCountAtStart, isBlue: true);

            loop = true;
            isBlue = true;
        }

        public override void ReportMonsterDroneHitByPlayer(MonsterController monsterDrone)
        {
            base.ReportMonsterDroneHitByPlayer(monsterDrone);

            progress++;

            OnProgressUpdated?.Invoke(progress, goal);

            if (progress >= goal)
            {
                loop = false;
                GameManager.Instance.ReportPlayerIsWinner();
            }
        }

        void UpdateSwitchTime()
        {
            switchElapsed += Time.deltaTime;

            if (switchElapsed > switchTime)
            {
                switchElapsed -= switchTime;

                // Switch
                PlayerController.Instance.SetState(isBlue ? PlayerState.Hunter : PlayerState.Prey);
                isBlue = !isBlue;

                spawnElapsed = 0;

            }
        }

        void UpdateSpawnTime()
        {
            spawnElapsed += Time.deltaTime;

            if (spawnElapsed > spawnTime)
            {
                spawnElapsed -= spawnTime;

                MonsterSpawner.Instance.SpawnRandomMonsters(spawnAmount, isBlue);
            }
        }

        

        public float GetTimeLeft()
        {
            float ret = switchTime - switchElapsed;
            if (ret < 0) ret = 0;
            return ret;
        }
    }
}