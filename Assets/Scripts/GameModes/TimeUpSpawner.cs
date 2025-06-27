using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class TimeUpSpawner : Singleton<TimeUpSpawner>
    {
        public delegate void OnTimeUpSpawnedDelegate(GameObject timeUp);
        public static OnTimeUpSpawnedDelegate OnTimeUpSpawned;
        
        public delegate void OnTimeUpUnspawnedDelegate(GameObject timeUp);
        public static OnTimeUpUnspawnedDelegate OnTimeUpUnspawned;

        [SerializeField]
        GameObject timeUpPrefab;

        GameObject timeUp;

        bool spawning = false;

        float spawnTime = 5;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (!spawning) return;

        }

        public async void StartSpawner()
        {
            spawning = true;

            await Task.Delay(TimeSpan.FromSeconds(spawnTime));

            if (!spawning) return;

            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > 18);
            var position = candidates[UnityEngine.Random.Range(0, candidates.Count)].position;
            timeUp = Instantiate(timeUpPrefab, position, Quaternion.identity);


            
            OnTimeUpSpawned?.Invoke(timeUp);
        }

        

        public void StopSpawner()
        {
            spawning = false;
            if (timeUp)
            {
                OnTimeUpUnspawned?.Invoke(timeUp);
                Destroy(timeUp);
                
            }
                

        }

        public void ReportTimeUpPicked()
        {
            StopSpawner();
            StartSpawner();
        }
    }
}