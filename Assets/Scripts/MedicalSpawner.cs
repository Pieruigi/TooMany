using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            GameManager.OnStateChanged += HandleOnGameStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        private void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Winner:
                case GameState.Loser:
                    UnspawnMedicalDrone();
                    break;
            }
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

        public void ForceSpawnMedicalDrone()
        {
            SpawnMedicalDrone().Forget();
        }

        async UniTaskVoid SpawnMedicalDrone()
        {
            if (GameManager.Instance.NoPowerUps) return;
            
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));

            if (medicalDrone) return;

            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > TimeUpSpawner.PlayerMinDistance);
            // Get a random waypoint
            var waypoint = candidates[UnityEngine.Random.Range(0, LevelController.Instance.Waypoints.Count)];

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