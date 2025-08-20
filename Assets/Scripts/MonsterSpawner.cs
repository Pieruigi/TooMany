using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT
{
  
    public class MonsterSpawner : Singleton<MonsterSpawner>
    {
        public delegate void OnMonsterAddedDelegate(GameObject monster);
        public static OnMonsterAddedDelegate OnMonsterAdded;

        public delegate void OnMonsterRemovedDelegate(GameObject monster);
        public static OnMonsterRemovedDelegate OnMonsterRemoved;

        public delegate void OnSpawnCompletedDelegate(int amount);
        public static OnSpawnCompletedDelegate OnSpawnCompleted;

        [SerializeField]
        List<GameObject> monsterPrefabs;

        [SerializeField]
        int spawnAmount = 8;
        public int SpawnAmount
        {
            get { return spawnAmount; }
            set { spawnAmount = value; }
        }


        // [SerializeField]
        // List<Transform> spawnPoints;

        List<MonsterController> monsters = new List<MonsterController>();
        public IList<MonsterController> Monsters
        {
            get { return monsters; }
        }

        float spawnDistance = 20;

        bool spawnDisabled = false;
        float spawnTime = 40;
        public float SpawnTime
        {
            get { return spawnTime; }
            set { spawnTime = value; }
        }

        float spawnElapsed = 0;




        // Start is called before the first frame update
        void Start()
        {
            //SpawnRandomMonsters(spawnAmount);
        }

        // Update is called once per frame
        void Update()
        {
            if (spawnDisabled) return;

            spawnElapsed += Time.deltaTime;
            if (spawnElapsed > spawnTime)
            {
                spawnElapsed -= spawnTime;
                SpawnRandomMonsters(spawnAmount);
            }

        }

        void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnStateChanged;
        }

        void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Winner:
                    DestroyAllDrones();
                    break;
            }


        }

        void DestroyAllDrones()
        {
            List<MonsterController> toDestroy = new List<MonsterController>();
            // Destroy all drones
            foreach (var monster in monsters)
                toDestroy.Add(monster);

            // Clear list
            monsters.Clear();

            foreach (var monster in toDestroy)
                monster.SetState(MonsterState.Dying);
        }


        public void SpawnRandomMonsters(int count, bool isBlue = false)
        {
            //return;
            List<Transform> candidates = LevelController.Instance.Waypoints.ToList().FindAll(s => Vector3.Distance(PlayerController.Instance.transform.position, s.position) > spawnDistance);
            for (int i = 0; i < count; i++)
            {
                // Get a random spawn point
                var sp = candidates[Random.Range(0, candidates.Count)];
                // Remove spawn point from candidates
                candidates.Remove(sp);
                // Get a random monster prefab
                var mp = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
                // Spawn new monster
                var m = Instantiate(mp, sp.position, sp.rotation);
                // Add to the nonster list
                monsters.Add(m.GetComponent<MonsterController>());

                // Set behaviour
                m.GetComponent<MonsterController>().InvertedBehaviour = isBlue;

                OnMonsterAdded?.Invoke(m);
            }

            OnSpawnCompleted?.Invoke(count);
        }

        public void DestroyMonsterDelayed(MonsterController monster, float delay)
        {
            monsters.Remove(monster);
            OnMonsterRemoved?.Invoke(monster.gameObject);
            Destroy(monster.gameObject, delay);


        }

        public void StopSpawner()
        {
            spawnDisabled = true;
        }

        public void StartSpawner()
        {
            spawnDisabled = false;
            spawnElapsed = 0;
        }



    }
    
}
