using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace TMOT.UI
{
    public class GameMode1_UI : GameModeUI
    {
        

        [SerializeField]
        TMP_Text goalTimerField;

     
        [SerializeField]
        TMP_Text chaseTimerField;

        [SerializeField]
        TMP_Text switchField;

        string timeStringFormat = "{0:00}:{1:00}";


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            switch (GameManager.Instance.GameState)
            {
                case GameState.Playing:
                    UpdatePlayingState();
                    UpdateSwitchTimer();
                    break;
               
            }


        }

        

        protected override void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            base.HandleOnGameStateChanged(oldState, newState);
            switch (newState)
            {
                case GameState.Starting:
                    ShowChaseTimer(false);
                    UpdateGoalTimer();
                    ShowSwitchTimer(false);
                    break;

            }
        }


        void UpdatePlayingState()
        {
            UpdateGoalTimer();
        }


        void UpdateSwitchTimer()
        {
            var timeLeft = (GameMode.Instance as GameMode1).GetSwitchTimeLeft();
            if (timeLeft < 5)
            {
                ShowSwitchTimer(true);
                switchField.text = Mathf.FloorToInt(timeLeft).ToString();
            }
            else
            {
                ShowSwitchTimer(false);
            }

        }

        void UpdateGoalTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetGoalTimeRemaining();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            goalTimerField.text = string.Format(timeStringFormat, minutes, seconds);
        }

       
        void ShowChaseTimer(bool value)
        {
            chaseTimerField.gameObject.SetActive(value);
        }

        void ShowSwitchTimer(bool value)
        {
            if (switchField.gameObject.activeSelf == value) return;
            switchField.gameObject.SetActive(value);
        }

        

    }
}