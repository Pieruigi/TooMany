using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace TMOT
{
    public class MonsterSpawner : Singleton<MonsterSpawner>
    {
        [SerializeField]
        List<GameObject> monsterPrefabs;

        [SerializeField]
        int initialNumber = 8;

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

        float spawnElapsed = 0;



        // Start is called before the first frame update
        void Start()
        {
            SpawnRandomMonsters(initialNumber);

            //StartCoroutine(_Test());
        }

        // Update is called once per frame
        void Update()
        {
            if (spawnDisabled) return;

            spawnElapsed += Time.deltaTime;
            if (spawnElapsed > spawnTime)
            {
                spawnElapsed -= spawnTime;
                SpawnRandomMonsters(initialNumber);
            }

        }

        IEnumerator _Test()
        {
            // TEST - Level controller should call SpawnRandomMonsters()
            while (true)
            {
                yield return new WaitForSeconds(20);
                SpawnRandomMonsters(initialNumber);
            }
        }

        public void SpawnRandomMonsters(int count)
        {
            Debug.Log("TEST - spawn new monsters");

            List<Transform> candidates = WayPointManager.Instance.WayPoints.ToList().FindAll(s => Vector3.Distance(PlayerController.Instance.transform.position, s.position) > spawnDistance);
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
            }
        }

        public void DestroyMonsterDelayed(MonsterController monster, float delay)
        {
            monsters.Remove(monster);
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
