using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMOT.UI
{
    public abstract class GameModeUI : MonoBehaviour
    {

        [SerializeField]
        GameObject loserPanel;

        [SerializeField]
        GameObject winnerPanel;

        [SerializeField]
        GameObject buttonPanel;

        [SerializeField]
        Button restartButton, quitButton;

        

        [SerializeField]
        TMP_Text goalField;

        protected virtual void Awake()
        {
            loserPanel.gameObject.SetActive(false);
            winnerPanel.gameObject.SetActive(false);
            buttonPanel.SetActive(false);

            restartButton.onClick.AddListener(RestartGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        protected virtual void Start()
        {
            // Setting overlay camera
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            //canvas.worldCamera = Camera.main.GetUniversalAdditionalCameraData().cameraStack[0];
        }

        protected virtual void Update() { }

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

                case GameState.Loser:
                    ShowLoserPanel().Forget();
                    break;
                case GameState.Winner:
                    ShowWinnerPanel().Forget();
                    break;
            }
        }



        async UniTaskVoid ShowLoserPanel()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(4f));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Time.timeScale = 0;

            loserPanel.gameObject.SetActive(true);
            buttonPanel.SetActive(true);
            
        }

        async UniTaskVoid ShowWinnerPanel()
        {

            await UniTask.Delay(TimeSpan.FromSeconds(4f));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Time.timeScale = 0;

            winnerPanel.gameObject.SetActive(true);
            buttonPanel.SetActive(true);
        }


        public void RestartGame()
        {
            Time.timeScale = 1;
            GameManager.Instance.PlayGame();
            
        }

        public void QuitGame()
        {
            GameManager.Instance.LoadMainScene();
        }

        public void UpdateGoal(string text)
        {
            goalField.text = text;
        }
        
    }
}