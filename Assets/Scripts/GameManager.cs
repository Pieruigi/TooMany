using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace TMOT
{
    public enum GameState { None, Starting, Playing, Paused, Loser, Winner }

    public enum GameModeType { Mode1, Mode2, Mode3 }

    public class GameManager : SingletonPersistent<GameManager>
    {
        public delegate void OnStateChangedDelegate(GameState oldState, GameState newState);
        public static OnStateChangedDelegate OnStateChanged;

        public const float StartingDelay = 1.5f;

        GameState gameState = GameState.None;
        public GameState GameState
        {
            get { return gameState; }
        }

        GameModeType gameMode = GameModeType.Mode1;
        public GameModeType GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }       
        }

        int mapId = 0;

        int gameSceneOffset = 1;

        int mainSceneIndex = 0;

        protected override void Awake()
        {
            base.Awake();



#if UNITY_EDITOR && UNITY_WEBGL
            PlayerSettings.WebGL.threadsSupport = true; // When enabled, Unity generates a WebGL build with multithreading support enabled.

            Application.targetFrameRate = -1;
#endif
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
                //SceneManager.LoadScene(1);
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
                    EnterStartingState().Forget();
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

        async UniTaskVoid EnterStartingState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            LevelController.Instance.Initialize();
          
            await UniTask.Delay(TimeSpan.FromSeconds(StartingDelay));
          
            SetState(GameState.Playing);

           
        }

        void EnteringLoserState()
        {

        }

        void EnteringWinnerState()
        {
            // Check if the player unlocked a new game mode
            if (gameMode == (GameModeType) SaveManager.Instance.GameProgress)
                SaveManager.Instance.UpdateGameProgress();
            
        }

        public void ReportPlayerIsWinner()
        {
            SetState(GameState.Winner);
        }

        public void ReportPlayerIsLoser()
        {
            SetState(GameState.Loser);
        }

        public void PlayGame()
        {
            Debug.Log("TEST - PlatGame()");
            SceneManager.LoadScene(gameSceneOffset + mapId);
        }

        public void LoadMainScene()
        {
             SceneManager.LoadScene(mainSceneIndex);
        }
    }
}