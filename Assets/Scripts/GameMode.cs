using System.Collections;
using System.Collections.Generic;
using TMOT.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TMOT
{
    public abstract class GameMode : Singleton<GameMode>
    {
        public delegate void ProgressUpdatedDelegate(int progress, int goal);
        public static ProgressUpdatedDelegate OnProgressUpdated;


        [SerializeField]
        GameObject gameUIPrefab;

        [SerializeField]
        bool startInHuntingMode = false;
        public bool StartInHuntingMode
        {
            get { return startInHuntingMode; }
        }



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

        public virtual void ReportCustomDronePicked(CustomDroneController customDrone)
        {

        }

        public virtual void ReportMonsterDroneHitByPlayer(MonsterController monsterDrone)
        {
            
        }

    }
}