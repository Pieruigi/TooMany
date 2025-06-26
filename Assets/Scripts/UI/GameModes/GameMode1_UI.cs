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

        [SerializeField]
        TMP_Text switchField;

        string timeStringFormat = "{0:00}:{1:00}";

        Animator chaseTimerAnimator, goalTimerAnimator;

        bool switching = false;

        string switchHunterTxt = "Switch to killer in {0:00}";
        string switchPreyTxt = "Switch to victim in {0:00}";

        protected override void Awake()
        {
            base.Awake();

            chaseTimerAnimator = chaseTimerField.GetComponent<Animator>();
            goalTimerAnimator = goalTimerField.GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateSwitchText();
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
                    //UpdateSwitchTimer();
                    UpdateMonsterCounter();
                    UpdateSwitchText();
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

        protected override void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            base.HandleOnPlayerStateChanged(oldState, newState);

            switch (newState)
            {
                case PlayerState.Prey:
                    ShowTimer(chaseTimerAnimator, false, init:true);
                    ShowTimer(goalTimerAnimator, true, init:true);
                    break;
                case PlayerState.Hunter:
                    ShowTimer(chaseTimerAnimator, true, init:true);
                    ShowTimer(goalTimerAnimator, false, init:true);
                    break;
            }
        }


        void UpdatePlayingState()
        {
            UpdateGoalTimer();
        }

        void UpdateSwitchText()
        {
            var timeLeft = (GameMode.Instance as GameMode1).GetSwitchTimeLeft();
            string s = PlayerController.Instance.State == PlayerState.Hunter ? string.Format(switchPreyTxt, timeLeft) : string.Format(switchHunterTxt, Mathf.CeilToInt(timeLeft));
            switchField.text = s;
        }

        // void UpdateSwitchTimer()
        // {
        //     var timeLeft = (GameMode.Instance as GameMode1).GetSwitchTimeLeft();
        //     if (timeLeft < 3)
        //     {
        //         if (!switching)
        //         {
        //             switching = true;
        //             ShowTimer(chaseTimerAnimator, PlayerController.Instance.State == PlayerState.Prey ? true : false);
        //             ShowTimer(goalTimerAnimator, PlayerController.Instance.State == PlayerState.Prey ? false : true);
        //         }

        //     }
        //     else
        //     {
        //         if (switching)
        //             switching = false;
        //     }

        // }

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