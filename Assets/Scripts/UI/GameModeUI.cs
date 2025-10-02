using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        Vector3 goalFieldLocalPositionDefault;



        protected virtual void Awake()
        {
            loserPanel.gameObject.SetActive(false);
            winnerPanel.gameObject.SetActive(false);
            buttonPanel.SetActive(false);

            restartButton.onClick.AddListener(RestartGame);
            quitButton.onClick.AddListener(QuitGame);

            goalFieldLocalPositionDefault = goalField.transform.localPosition;
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
            GameMode.OnProgressUpdated += HandleOnProgressUpdated;
            GameMenuUI.OnGameMenuVisible += HandleOnGameMenuVisible;
        }



        protected virtual void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
            GameMode.OnProgressUpdated -= HandleOnProgressUpdated;
            GameMenuUI.OnGameMenuVisible -= HandleOnGameMenuVisible;
        }

        private void HandleOnGameMenuVisible(bool visible)
        {
            GetComponent<CanvasGroup>().alpha = visible ? 0 : 1;
        }

        private void HandleOnProgressUpdated(int progress, int goal)
        {
            UpdateGoal($"{progress}/{goal}");
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
            await UniTask.Delay(TimeSpan.FromSeconds(4f*GameManager.Instance.GameSpeed));

            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
            //Time.timeScale = 0;

            loserPanel.gameObject.SetActive(true);
            //buttonPanel.SetActive(true);

        }

        async UniTaskVoid ShowWinnerPanel()
        {

            await UniTask.Delay(TimeSpan.FromSeconds(2f*GameManager.Instance.GameSpeed));

            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
            //Time.timeScale = 0;

            winnerPanel.gameObject.SetActive(true);
            //buttonPanel.SetActive(true);
        }

        protected async UniTaskVoid ShowPlayerStateChangedMessage(CanvasGroup canvasGroup)
        {
            float duration = .2f;
            float shakeDuration = .5f;
            float strength = 20;
            canvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad);
            canvasGroup.transform.DOShakePosition(shakeDuration, strength).SetEase(Ease.InOutElastic);

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            canvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad);
            canvasGroup.transform.DOShakePosition(shakeDuration, strength).SetEase(Ease.InOutElastic);
        
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

            goalField.transform.DOShakePosition(.5f, 30f).SetEase(Ease.InOutElastic).onComplete += ()=> { goalField.transform.localPosition = goalFieldLocalPositionDefault; };
        }

        
        
    }
}