using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class PillSpawner : Singleton<PillSpawner>
    {
        public delegate void SpawnedDelegate(GameObject drone);
        public static SpawnedDelegate OnSpawned;

        public delegate void UnspawnedDelegate(GameObject drone);
        public static UnspawnedDelegate OnUnspawned;

        [SerializeField]
        GameObject prefab;

        GameObject drone;

        int spawnChance = 30;

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
                SpawnDrone().Forget();
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
                    UnspawnDrone();
                    break;
            }
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            if (newState == PlayerState.Prey)
            {
                bool spawn = UnityEngine.Random.Range(0, 100) < spawnChance;

                if (spawn)
                {
                    SpawnDrone().Forget();
                }
            }
            else
            {
                UnspawnDrone();
            }
        }

        async UniTaskVoid SpawnDrone()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
            // Get a random waypoint
            var waypoint = LevelController.Instance.Waypoints[UnityEngine.Random.Range(0, LevelController.Instance.Waypoints.Count)];

            // Spawn medical
            drone = Instantiate(prefab, waypoint.position, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 350f), 0f));

            OnSpawned?.Invoke(drone);
        }

        void UnspawnDrone()
        {
            if (!drone) return;

            drone.GetComponent<CustomDroneController>().ForceDestroy();

            OnUnspawned?.Invoke(drone);

            Destroy(drone, 2f);
        }

        public void ReportPicked()
        {
            UnspawnDrone();
        }

    }
}