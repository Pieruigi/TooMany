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
        TMP_Text monsterCounterField;

        string timeStringFormat = "{0:00}:{1:00}";

        Animator chaseTimerAnimator, goalTimerAnimator;

        bool switching = false;

        protected override void Awake()
        {
            base.Awake();

            chaseTimerAnimator = chaseTimerField.GetComponent<Animator>();
            goalTimerAnimator = goalTimerField.GetComponent<Animator>();
        }

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
                    UpdateChaseTimer();
                    UpdateSwitchTimer();
                    UpdateMonsterCounter();
                    break;
               
            }


        }

        

        protected override void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            base.HandleOnGameStateChanged(oldState, newState);
            switch (newState)
            {
                case GameState.Starting:
                    ShowTimer(chaseTimerAnimator, false, init:true);
                    ShowTimer(goalTimerAnimator, true, init:true);
                    UpdateGoalTimer();
                    UpdateChaseTimer();
                    UpdateMonsterCounter();
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
            if (timeLeft < 3)
            {
                if (!switching)
                {
                    switching = true;
                    ShowTimer(chaseTimerAnimator, PlayerController.Instance.State == PlayerState.Prey ? true : false);
                    ShowTimer(goalTimerAnimator, PlayerController.Instance.State == PlayerState.Prey ? false : true);
                }

            }
            else
            {
                if (switching)
                    switching = false;
            }

        }

        void UpdateMonsterCounter()
        {
            monsterCounterField.text = MonsterSpawner.Instance.Monsters.Count.ToString();
        }

        void UpdateGoalTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetGoalTimeRemaining();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            goalTimerField.text = string.Format(timeStringFormat, minutes, seconds);
        }

        void UpdateChaseTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetChasingTimeLeft();
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            chaseTimerField.text = string.Format(timeStringFormat, minutes, seconds);
        }

        void ShowTimer(Animator animator, bool value, bool init = false)
        {
            string state = value ? "On" : "Off";
            if (init)
            {
                animator.Play(state, 0, 1.0f);
                animator.Update(0);
            }
            else
            {
                animator.SetTrigger(state);
            }
        }

   

        

    }
}