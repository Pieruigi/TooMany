using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class GameMode2 : GameMode
    {

        [SerializeField]
        DiamondSpawner diamondSpawnerPrefab;

        [SerializeField]
        int goalCount = 30;

        [SerializeField]
        int stepCount = 5;

        DiamondSpawner diamondSpawner;


        protected override void Awake()
        {
            base.Awake();

            // Spawn diamond spawner
            diamondSpawner = Instantiate(diamondSpawnerPrefab);

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    SpawnDiamonds().Forget();
                    break;
            }
        }

        protected override void StartGameMode()
        {
            PlayerController.Instance.SetState(PlayerState.Prey);
        }

        async UniTaskVoid SpawnDiamonds()
        {
            int count = goalCount / stepCount;

            for (int i = 0; i < count; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(.25f));

                diamondSpawner.SpawnDiamond();
            }
        }


    }
}