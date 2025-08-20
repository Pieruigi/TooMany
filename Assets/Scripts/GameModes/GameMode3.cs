using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode3 : GameMode
    {
        [SerializeField]
        GameObject monsterSpawnerPrefab;

        int blueCountAtStart = 14;
        int redCountAtStart = 14;

        protected override void Awake()
        {
            base.Awake();

            // Instantiate bot spawner
            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void StartGameMode()
        {
            Debug.Log("TEST - Starting game mode...");
            // Set player state
            PlayerController.Instance.SetState(PlayerState.Prey);

            // Spawn blue and red bots
            MonsterSpawner.Instance.SpawnRandomMonsters(blueCountAtStart);
            MonsterSpawner.Instance.SpawnRandomMonsters(blueCountAtStart, isBlue: true);
            
        }
    }
}