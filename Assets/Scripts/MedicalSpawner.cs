using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class MedicalSpawner : Singleton<MedicalSpawner>
    {
        public delegate void MedicalDroneSpawnedDelegate(GameObject drone);
        public static MedicalDroneSpawnedDelegate OnMedicalDroneSpawned;

        public delegate void MedicalDroneUnspawnedDelegate(GameObject drone);
        public static MedicalDroneUnspawnedDelegate OnMedicalDroneUnspawned;

        [SerializeField]
        GameObject prefab;

        GameObject medicalDrone;

        int spawnChance = 40;

        float spawnDelay = 1f;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
            {
                SpawnMedicalDrone().Forget();
            }
#endif
        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            if (newState == PlayerState.Prey)
            {
                if (!PlayerController.Instance.IsWounded()) return;

                // If player is wounded there is a chance a medical drone is spawned
                bool spawn = UnityEngine.Random.Range(0, 100) < spawnChance;

                if (spawn)
                {
                    SpawnMedicalDrone().Forget();
                }
            }
            else
            {
                UnspawnMedicalDrone();
            }
        }

        async UniTaskVoid SpawnMedicalDrone()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
            // Get a random waypoint
            var waypoint = LevelController.Instance.Waypoints[UnityEngine.Random.Range(0, LevelController.Instance.Waypoints.Count)];

            // Spawn medical
            medicalDrone = Instantiate(prefab, waypoint.position, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 350f), 0f));

            OnMedicalDroneSpawned?.Invoke(medicalDrone);
        }

        void UnspawnMedicalDrone()
        {
            if (!medicalDrone) return;

            medicalDrone.GetComponent<CustomDroneController>().ForceDestroy();

            OnMedicalDroneUnspawned?.Invoke(medicalDrone);

            Destroy(medicalDrone, 2f);
        }

        public void ReportMedicalPicked()
        {
            UnspawnMedicalDrone();
        }

      
    }
}