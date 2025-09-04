using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

namespace TMOT
{
    public class TimeUpMultiSpawner : Singleton<TimeUpMultiSpawner>
    {

        public delegate void TimeUpMultiSpawnedDelegate(List<GameObject> list);
        public static TimeUpMultiSpawnedDelegate OnTimeUpMultiSpawned;

           

        [SerializeField]
        GameObject timeUpPrefab;

        List<GameObject> timeUps = new List<GameObject>();

        bool spawning = false;

        float spawnTime = 2;

        int maxTimeUps = 5;

        async UniTaskVoid ReplaceTimeUp()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(spawnTime));

            if (!spawning) return;

            // Get all waypoints at a minimum distance from the player
            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > TimeUpSpawner.PlayerMinDistance);

            // Get a random waypoint
            var waypoint = candidates[UnityEngine.Random.Range(0, candidates.Count)];

            // Instantiate a new object
            var tup = Instantiate(timeUpPrefab, waypoint.position, Quaternion.identity);

            // Add the object to the list
            timeUps.Add(tup);

            TimeUpSpawner.OnTimeUpSpawned?.Invoke(tup);
        }

        public async UniTaskVoid StartSpawner()
        {
            if (spawning) return;

            spawning = true;

            await UniTask.Delay(TimeSpan.FromSeconds(spawnTime));

            if (!spawning) return;

            // Get all waypoints at a minimum distance from the player
            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > TimeUpSpawner.PlayerMinDistance);

            for (int i = 0; i < maxTimeUps; i++)
            {
                // Get a random waypoint
                var waypoint = candidates[UnityEngine.Random.Range(0, candidates.Count)];

                // Instantiate a new object
                var tup = Instantiate(timeUpPrefab, waypoint.position, Quaternion.identity);

                // Add the object to the list
                timeUps.Add(tup);

                // Remove the waypoint from candidates
                candidates.Remove(waypoint);

                candidates.RemoveAll(w => Vector3.Distance(w.position, waypoint.position) < TimeUpSpawner.PlayerMinDistance);

            }

            OnTimeUpMultiSpawned?.Invoke(timeUps);

        }

        public void StopSpawner()
        {
            if (!spawning) return;

            spawning = false;

            foreach (GameObject g in timeUps)
            {
                g.GetComponent<CustomDroneController>().ForceDestroy();
                Destroy(g, 2f);
                TimeUpSpawner.OnTimeUpUnspawned?.Invoke(g);
            }

            timeUps.Clear();

            
        }

        public void ReportTimeUpPicked(GameObject timeUp)
        {
            // Remove time up from the list
            timeUps.Remove(timeUp);

            // Destroy object
            Destroy(timeUp, 2f);

            ReplaceTimeUp().Forget();

            TimeUpSpawner.OnTimeUpUnspawned?.Invoke(timeUp);
        }


    }
}