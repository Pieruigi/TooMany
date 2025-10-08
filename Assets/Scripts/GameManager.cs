using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace TMOT
{
    public enum GameState { None, Starting, Playing, Paused, Loser, Winner }

    public enum GameModeType { Mode1, Mode2, Mode3, Mode4, Mode5 }

    public class GameManager : SingletonPersistent<GameManager>
    {
        public delegate void SceneLoadStartedDelegate(int sceneBuildIndex);
        public static SceneLoadStartedDelegate OnSceneLoadStarted;

        public delegate void SceneLoadCompletedDelegate();
        public static SceneLoadCompletedDelegate OnSceneLoadCompleted;

        public delegate void SceneLoadingProgressDelegate(float progress);
        public static SceneLoadingProgressDelegate OnSceneLoadingProgress;

        public delegate void OnStateChangedDelegate(GameState oldState, GameState newState);
        public static OnStateChangedDelegate OnStateChanged;

        public const float StartingDelay = 3f;

        public const int PowerUpSpawnChance = 10;
       

        [SerializeField]
        List<GameObject> gameModePrefabs;
        public ICollection<GameObject> GameModePrefabs
        {
            get{ return gameModePrefabs.AsReadOnly(); }
        }

        GameState gameState = GameState.None;
        public GameState GameState
        {
            get { return gameState; }
        }

        GameModeType gameMode = GameModeType.Mode1;
        public GameModeType GameMode
        {
            get { return gameMode; }
            set { gameMode = value; UpdateStage(); }
        }

        int mapId = 0;
        public int MapId
        {
            get { return mapId; }
            set { mapId = value; UpdateStage();}
        }

        int gameSceneOffset = 1;

        int mainSceneIndex = 0;

        float gameSpeed = 1f;
        public float GameSpeed
        {
            get { return gameSpeed; }
            //set { gameSpeed = value; }
        }

        

        float speedUpStep = .15f;

        int gameStage = 0;
        public int GameStage
        {
            get { return gameStage; }
            set { gameStage = value; }
        }


        float restartTime = 5;


    

#if UNITY_EDITOR
        bool noPowerUps = false;
#else
        bool noPowerUps = false;
#endif

        public bool NoPowerUps
        {
            get { return noPowerUps; }
        }
        

        AsyncOperation loadingOperation;

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
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
                ReportPlayerIsWinner();
#endif
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
                gameSpeed = 1;
                gameStage = Mathf.Min(StageManager.MaxStage, StageManager.GetLastStage(mapId, gameMode));
                
                Debug.Log($"GameManager - GameStage:{gameStage}");

#if UNITY_EDITOR
                //gameStage = 12;
#endif
                SetState(GameState.None);
                //SceneManager.LoadScene(1);
            }
            else // Game scene
            {

                SetState(GameState.Starting);
            }
        }

        void UpdateStage()
        {
            gameStage = Mathf.Min(StageManager.MaxStage, StageManager.GetLastStage(mapId, gameMode));
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
                    EnteringLoserState().Forget();
                    break;
                case GameState.Winner:
                    EnteringWinnerState().Forget();
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);
        }

        async UniTaskVoid EnterStartingState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            LevelController.Instance.Initialize();

            await UniTask.Delay(TimeSpan.FromSeconds(StartingDelay*gameSpeed));

            SetState(GameState.Playing);


        }

        async UniTaskVoid EnteringLoserState()
        {
            
            await UniTask.Delay(TimeSpan.FromSeconds((restartTime+2f)*gameSpeed));
            gameSpeed = 1; // Reset game speed since you lose
            //gameStage = 0;
            PlayGame();

        }

        async UniTaskVoid EnteringWinnerState()
        {
#if !DEMO
            //if (gameSpeed == 1)
            Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
            if (gameStage == 0)
            {
                // Check if the player unlocked a new game mode
                if (gameMode == (GameModeType)SaveManager.Instance.GameProgress)
                    SaveManager.Instance.UpdateGameProgress();

                
            }
            // Try unlock achievement
            //SteamAchievementManager.Instance.UnlockAchievement($"BEAT_GM_{(int)gameMode+1}");
            StageManager.UpdateLastStage(mapId, gameMode, gameStage+1); 

            await UniTask.Delay(TimeSpan.FromSeconds(restartTime * gameSpeed));
            //gameSpeed += speedUpStep;
            gameStage++;
            
            
            PlayGame();
#else
            await UniTask.Delay(TimeSpan.FromSeconds(restartTime * gameSpeed));
            if (gameSpeed == 1)
            {
                gameSpeed += speedUpStep;
                PlayGame();
            }
            else // Cap
            {
                await DemoManager.Instance.Show(4,1);
                gameSpeed = 1;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                LoadMainScene();
            }
#endif



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
            StartCoroutine(LoadSceneAsync(gameSceneOffset + mapId));
        }

        IEnumerator LoadSceneAsync(int sceneBuildIndex)
        {
            OnSceneLoadStarted?.Invoke(sceneBuildIndex);

            Time.timeScale = 1;
            loadingOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);

            while (!loadingOperation.isDone)
            {
                OnSceneLoadingProgress?.Invoke(loadingOperation.progress);
                yield return null;
            }

            OnSceneLoadCompleted?.Invoke();
        }

        public void LoadMainScene()
        {
            //SceneManager.LoadScene(mainSceneIndex);
            StartCoroutine(LoadSceneAsync(mainSceneIndex));
        }

        public void PauseGame()
        {
            gameState = GameState.Paused;
            Time.timeScale = 0;
            PlayerController.Instance.InputDisabled = true;
        }

        public void ResumeGame()
        {
            gameState = GameState.Playing;
            Time.timeScale = gameSpeed;
            PlayerController.Instance.InputDisabled = false;
        }
    }
}