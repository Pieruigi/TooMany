using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class TimeUpSpawner : Singleton<TimeUpSpawner>
    {
        public const float PlayerMinDistance = 18;

        public delegate void OnTimeUpSpawnedDelegate(GameObject timeUp);
        public static OnTimeUpSpawnedDelegate OnTimeUpSpawned;
        
        public delegate void OnTimeUpUnspawnedDelegate(GameObject timeUp);
        public static OnTimeUpUnspawnedDelegate OnTimeUpUnspawned;

        [SerializeField]
        GameObject timeUpPrefab;

        GameObject timeUp;

        bool spawning = false;

        float spawnTime = 2;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (!spawning) return;

        }

        public async UniTaskVoid StartSpawner()
        {
            spawning = true;

            await UniTask.Delay(TimeSpan.FromSeconds(spawnTime));

            if (!spawning) return;

            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > PlayerMinDistance);
            var position = candidates[UnityEngine.Random.Range(0, candidates.Count)].position;
            timeUp = Instantiate(timeUpPrefab, position, Quaternion.identity);


            
            OnTimeUpSpawned?.Invoke(timeUp);
        }

        

        public void StopSpawner()
        {
            spawning = false;
            if (timeUp)
            {
                // We must forse explosion here, otherwise if we pick the power up the spawner will restart due to the ReportTimeUpPicked() call
                timeUp.GetComponent<CustomDroneController>().ForceDestroy();
                Destroy(timeUp,2f);
                OnTimeUpUnspawned?.Invoke(timeUp);
                
                
            }
                

        }

        public void ReportTimeUpPicked()
        {
            StopSpawner();
            StartSpawner().Forget();
        }
    }
}