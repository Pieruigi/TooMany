using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace TMOT.UI
{
    public class GameMenuUI : MonoBehaviour
    {
        public delegate void GameMenuVisibleDelegate(bool visible);
        public static GameMenuVisibleDelegate OnGameMenuVisible;
        

        [SerializeField]
        CanvasGroup panel;

        [SerializeField]
        GameObject actionKey;


        float fadeTime = .25f;

        void Awake()
        {
            panel.alpha = 0;
            panel.blocksRaycasts = false;

            UpdateActionKey();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.GameState != GameState.Playing && GameManager.Instance.GameState != GameState.Paused) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance.GameState == GameState.Playing)
                {
                    //Time.timeScale = 0;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    ShowPanel();
                }
                else
                {
                    HidPanel();
                    //Time.timeScale = GameManager.Instance.GameSpeed;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                }
            }

        }

        void UpdateActionKey()
        {
            if (GameManager.Instance.GameMode != GameModeType.Mode3 && GameManager.Instance.GameMode != GameModeType.Mode5)
                actionKey.SetActive(false);
        }

        void ShowPanel()
        {
            panel.DOFade(1, fadeTime).SetEase(Ease.InOutQuad).OnComplete(() => { GameManager.Instance.PauseGame(); });
            panel.blocksRaycasts = true;

            OnGameMenuVisible?.Invoke(true);
        }

        void HidPanel()
        {
            GameManager.Instance.ResumeGame();
            panel.DOFade(0, fadeTime).SetEase(Ease.InOutQuad);
            panel.blocksRaycasts = false;

            OnGameMenuVisible?.Invoke(false);
        }

        public void BackToMenu()
        {
            GameManager.Instance.LoadMainScene();
        }

        public void Continue()
        {
             HidPanel();
             Cursor.visible = false;
             Cursor.lockState = CursorLockMode.Locked;
        }
    }
}