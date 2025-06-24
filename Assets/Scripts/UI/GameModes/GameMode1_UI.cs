using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
                    break;
                
            }
        }


        void UpdatePlayingState()
        {
            UpdateGoalTimer();
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

        

        

    }
}