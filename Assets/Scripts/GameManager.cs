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
        }

        int mapId = 0;

        int gameSceneOffset = 1;

        int mainSceneIndex = 0;

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
            Debug.Log("TEST - Loading scene " + scene.name);
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
            Debug.Log("TEST - GameManager Delay before...");
            await Task.Delay(TimeSpan.FromSeconds(StartingDelay));
            Debug.Log("TEST - GameManager Delay after");
            SetState(GameState.Playing);
        }

        void EnteringLoserState()
        {

        }

        void EnteringWinnerState()
        {
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
            // Time.timeScale = 0;
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