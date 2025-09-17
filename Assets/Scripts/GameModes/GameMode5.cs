using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode5 : GameMode
    {

        [SerializeField]
        GameObject monsterSpawnerPrefab;

        [SerializeField]
        MedicalSpawner medicalSpawnerPrefab;

        [SerializeReference]
        PillSpawner pillSpawnerPrefab;

        int initialMonsterAmount = 20;

        int monsterAmount = 4;

        float monsterSpawnTime = 10;

        protected override void Awake()
        {
            base.Awake();

            Instantiate(monsterSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(medicalSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(pillSpawnerPrefab, Vector3.zero, Quaternion.identity);
            MonsterSpawner.Instance.StopSpawner();
            MonsterSpawner.Instance.SpawnTime = monsterSpawnTime;
            MonsterSpawner.Instance.SpawnAmount = monsterAmount;


        }

        protected override void StartGameMode()
        {
            // Set player
            PlayerController.Instance.SetState(PlayerState.Hunter);

            // Spawn the initial amount of monsters
            MonsterSpawner.Instance.SpawnRandomMonsters(initialMonsterAmount, true);

        }
    }
}