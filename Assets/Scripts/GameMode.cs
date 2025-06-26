using System.Collections;
using System.Collections.Generic;
using TMOT.UI;
using UnityEngine;

namespace TMOT
{
    public abstract class GameMode : Singleton<GameMode>
    {
        [SerializeField]
        GameObject gameUIPrefab;

        

        protected abstract void StartGameMode();

        //GameObject gameUI;

        protected override void Awake()
        {
            base.Awake();
            Instantiate(gameUIPrefab);

        }

    
        protected virtual void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
        }

        protected virtual void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        protected virtual void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    StartGameMode(); 
                    break;

            }
        }

    }
}