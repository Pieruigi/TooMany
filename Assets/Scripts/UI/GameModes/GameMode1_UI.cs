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
        TMP_Text preyTime;

        [SerializeField]
        TMP_Text hunterTime;



        string timeStringFormat = "{0:00}:{1:00}";

        CanvasGroup preyCanvasGroup, hunterCanvasGroup;       

        Color activatedColor = new Color(1, 1, 1, 1);
        Color deactivatedColor = new Color(0.5f, 0.5f, .5f, .25f);


        protected override void Awake()
        {
            base.Awake();
            preyCanvasGroup = preyTime.GetComponent<CanvasGroup>();
            hunterCanvasGroup = hunterTime.GetComponent<CanvasGroup>();
            hunterCanvasGroup.alpha = 0;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            UpdateTimer();

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            if (oldState == PlayerState.None) return;

            switch (newState)
            {
                case PlayerState.Prey:
                    preyCanvasGroup.alpha = 1;
                    hunterCanvasGroup.alpha = 0;
                    break;
                case PlayerState.Hunter:
                    preyCanvasGroup.alpha = 0;
                    hunterCanvasGroup.alpha = 1;
                    break;
            }
        }

        protected override void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            base.HandleOnGameStateChanged(oldState, newState);

            switch (newState)
            {
                case GameState.Playing:

                    break;
            }
        }

        void UpdateTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetTimeRemaining();

            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            if (PlayerController.Instance.State == PlayerState.Prey)
                preyTime.text = Mathf.CeilToInt(t).ToString();// string.Format(timeStringFormat, minutes, seconds);
            else if (PlayerController.Instance.State == PlayerState.Hunter)
                hunterTime.text = Mathf.CeilToInt(t).ToString(); //string.Format(timeStringFormat, minutes, seconds);

        }

        void UpdateGoalTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetGoalTimeRemaining();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            //goalTimerField.text = string.Format(timeStringFormat, minutes, seconds);
        }

        void UpdateChaseTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetChasingTimeLeft();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            //chaseTimerField.text = string.Format(timeStringFormat, minutes, seconds);
        }

     
        

        

    }
}