using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class BatterySpawner : Singleton<BatterySpawner>
    {
        public delegate void SpawnedDelegate(GameObject drone);
        public static SpawnedDelegate OnSpawned;

        public delegate void UnspawnedDelegate(GameObject drone);
        public static UnspawnedDelegate OnUnspawned;

        [SerializeField]
        GameObject prefab;

        GameObject drone;

        
        float spawnDelay = 1f;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
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
                bool spawn = Random.Range(0, 100) < GameManager.PowerUpSpawnChance;

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
            if (GameManager.Instance.NoPowerUps) return;

            await UniTask.Delay(System.TimeSpan.FromSeconds(spawnDelay));

            

            var candidates = LevelController.Instance.Waypoints.ToList().FindAll(w => Vector3.Distance(PlayerController.Instance.transform.position, w.position) > TimeUpSpawner.PlayerMinDistance);
            // Get a random waypoint
            var waypoint = candidates[UnityEngine.Random.Range(0, LevelController.Instance.Waypoints.Count)];

            // Spawn medical
            drone = Instantiate(prefab, waypoint.position, Quaternion.Euler(0f, Random.Range(0f, 350f), 0f));

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