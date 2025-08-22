using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode3 : GameMode
    {
        [SerializeField]
        GameObject monsterSpawnerPrefab;

        int goal = 30;

        int progress = 0;

        int stage = 0;

        float switchTime = 40;

        float switchElapsed = 0;

        bool loop = false;

        bool isBlue = false;

        int blueCountAtStart = 2;//14;
        int redCountAtStart = 2;//14;

        float spawnTime = 9;

        int spawnAmount = 4;
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

        }


        // Update is called once per frame
        void Update()
        {
            if (!loop) return;

            UpdateSwitchTime();

            UpdateSpawnTime();
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