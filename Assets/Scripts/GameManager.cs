using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace TMOT
{
    public enum GameState { None, Starting, Playing, Paused, Loser, Winner }

    public enum GameModeType { Mode1, Mode2 }

    public class GameManager : SingletonPersistent<GameManager>
    {
        public delegate void OnStateChangedDelegate(GameState oldState, GameState newState);
        public static OnStateChangedDelegate OnStateChanged;

        public const float StartingDelay = 5f;

        GameState gameState = GameState.None;
        public GameState GameState
        {
            get { return gameState; }
        }

        GameModeType gameMode = GameModeType.Mode1;
        public GameModeType GameMode
        {
            get { return gameMode; }
        }


        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR && UNITY_WEBGL
           PlayerSettings.WebGL.threadsSupport = true; // When enabled, Unity generates a WebGL build with multithreading support enabled.
#endif
        }

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
            SceneManager.sceneLoaded += HandleOnSceneLoaded;

        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0) // Menu
            {
                SetState(GameState.None);
            }
            else // Game scene
            {
                SetState(GameState.Starting);
            }
        }

        void SetState(GameState newState)
        {
            if (newState == gameState) return;
            var oldState = gameState;
            gameState = newState;

            switch (gameState)
            {
                case GameState.Starting:
                    EnterStartingState();
                    break;
                case GameState.Loser:
                    EnteringLoserState();
                    break;
                case GameState.Winner:
                    EnteringWinnerState();
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);
        }

        async void EnterStartingState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            LevelController.Instance.Initialize();
            await Task.Delay(TimeSpan.FromSeconds(StartingDelay));

            SetState(GameState.Playing);
        }

        void EnteringLoserState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }

        void EnteringWinnerState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }

        public async void ReportPlayerIsWinner()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            SetState(GameState.Winner);
        }

        public async void ReportPlayerIsLoser()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            SetState(GameState.Loser);
        }
    }
}