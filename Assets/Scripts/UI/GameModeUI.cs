using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        TMP_Text readyField;

        [SerializeField]
        TMP_Text killEveryoneField;

        [SerializeField]
        TMP_Text runAwayField;



        [SerializeField]
        GameObject loserPanel;

        [SerializeField]
        GameObject winnerPanel;


        float readyDelay = 0;
        float readyElapsed = 0;

        bool skipFirst = true;

        protected virtual void Awake()
        {
            killEveryoneField.gameObject.SetActive(false);
            runAwayField.gameObject.SetActive(false);
            loserPanel.gameObject.SetActive(false);
            winnerPanel.gameObject.SetActive(false);
        }

        protected virtual void Start()
        {
            // Setting overlay camera
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            //canvas.worldCamera = Camera.main.GetUniversalAdditionalCameraData().cameraStack[0];
        }

        // Update is called once per frame
        protected virtual void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            switch (GameManager.Instance.GameState)
            {
                case GameState.Starting:
                    UpdateReady();
                    break;
                case GameState.Playing:
                    
                    break;
                case GameState.Loser:
                    
                    break;

            }
        }

        protected virtual void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;

        }

        protected virtual void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;

        }

        protected virtual void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {

            switch (newState)
            {
                case PlayerState.Hunter:
                    ShowKillEveryone();
                    break;
                case PlayerState.Prey:
                    ShowRunAway();
                    break;

            }
        }

        protected virtual void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Starting:
                    ShowReady(true);

                    break;
                case GameState.Playing:
                    ShowReady(false);
                    break;
                case GameState.Loser:
                    ShowLoserPanel();
                    break;
                case GameState.Winner:
                    ShowWinnerPanel();
                    break;
            }
        }

        void ShowReady(bool value)
        {
            readyField.gameObject.SetActive(value);
            if (value)
            {
                readyElapsed = 0;
                readyDelay = GameManager.StartingDelay - 0.01f;
                readyField.text = Mathf.FloorToInt(readyDelay).ToString();
            }
        }

        async void ShowLoserPanel()
        {
            return;
            await Task.Delay(TimeSpan.FromSeconds(10.7f));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;

            loserPanel.gameObject.SetActive(true);
        }

        async void ShowWinnerPanel()
        {
            return;
            await Task.Delay(TimeSpan.FromSeconds(10.7f));

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;

            winnerPanel.gameObject.SetActive(true);
        }

       
        void UpdateReady()
        {
            readyElapsed += Time.deltaTime;
            var t = Mathf.FloorToInt(readyDelay - readyElapsed);
            if (t > 1)
            {
                readyField.text = (t - 1).ToString();
            }
            else
            {
                readyField.gameObject.SetActive(false);
                ShowRunAway();
            }

        }

        async void ShowKillEveryone()
        {
            if (skipFirst)
            {
                skipFirst = false;
                return;
            }
            killEveryoneField.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(2));
            killEveryoneField.gameObject.SetActive(false);
        }

        async void ShowRunAway()
        {
            if (skipFirst)
            {
                skipFirst = false;
                return;
            }
            runAwayField.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(2));
            runAwayField.gameObject.SetActive(false);
        }

        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
    }
}