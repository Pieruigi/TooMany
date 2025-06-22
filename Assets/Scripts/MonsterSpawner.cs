using System.Collections;
using System.Collections.Generic;
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
        int initialNumber = 4;

        [SerializeField]
        List<Transform> spawnPoints;

        List<MonsterController> monsters = new List<MonsterController>();
        public IList<MonsterController> Monsters
        {
            get{ return monsters; }
        }

        float spawnDistance = 20;





        // Start is called before the first frame update
        void Start()
        {
            SpawnRandomMonsters(initialNumber);

            StartCoroutine(_Test());
        }

        // Update is called once per frame
        void Update()
        {
            

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
            List<Transform> candidates = spawnPoints.FindAll(s => Vector3.Distance(PlayerController.Instance.transform.position, s.position) > spawnDistance);
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
        


    }
    
}
