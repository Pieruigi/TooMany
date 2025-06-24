using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public abstract class GameModeUI : MonoBehaviour
    {
        

        [SerializeField]
        TMP_Text readyField;


        float readyDelay = 0;
        float readyElapsed = 0;


        // Update is called once per frame
        protected virtual void Update()
        {
            switch (GameManager.Instance.GameState)
            {
                case GameState.Starting:
                    UpdateReady();
                    break;
            }
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
                case GameState.Starting:
                    ShowReady(true);

                    break;
                case GameState.Playing:
                    ShowReady(false);
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
        
        void UpdateReady()
        {
            readyElapsed += Time.deltaTime;
            var t = Mathf.FloorToInt(readyDelay - readyElapsed);
            if (t > 1)
                readyField.text = (t - 1).ToString();
            else
                readyField.text = "GO! GO! GO!";
        }
    }
}