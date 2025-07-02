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
        GameObject goalRoot;

        [SerializeField]
        GameObject chaseRoot;

        [SerializeField]
        GameObject switchRoot;


        [SerializeField]
        TMP_Text goalTimerField;

        [SerializeField]
        TMP_Text chaseTimerField;

        [SerializeField]
        TMP_Text monsterCounterField;

        [SerializeField]
        TMP_Text switchField;

        string timeStringFormat = "{0:00}:{1:00}";

       
        bool switching = false;

        string switchHunterTxt = "{0:00}";
        string switchPreyTxt = "{0:00}";

        Color activatedColor = new Color(1, 1, 1, 1);
        Color deactivatedColor = new Color(0.5f, 0.5f, .5f, .25f);

        Animator goalAnimator;
        Animator chaseAnimator;

        Animator switchAnimator;

        protected override void Awake()
        {
            base.Awake();
            goalAnimator = goalRoot.GetComponent<Animator>();
            chaseAnimator = chaseRoot.GetComponent<Animator>();
            switchAnimator = switchRoot.GetComponent<Animator>();

        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
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
                    chaseAnimator.SetTrigger("Off");

                    UpdateSwitchText();
                    UpdateGoalTimer();
                    UpdateChaseTimer();
                    UpdateMonsterCounter();
                    break;
                case GameState.Playing:
                    // goalAnimator.ResetTrigger("On");
                    // goalAnimator.ResetTrigger("Off");
                    // chaseAnimator.ResetTrigger("On");
                    // chaseAnimator.ResetTrigger("Off");

                    break;

            }
        }

        protected override void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            base.HandleOnPlayerStateChanged(oldState, newState);

            switch (newState)
            {
                case PlayerState.Prey:
                    if(!goalAnimator.GetCurrentAnimatorStateInfo(0).IsName("On"))
                        goalAnimator.SetTrigger("On");

                    if (!(GameMode1.Instance as GameMode1).IsLastStep())
                    {
                        if (!chaseAnimator.GetCurrentAnimatorStateInfo(0).IsName("Off"))
                            chaseAnimator.SetTrigger("Off");

                        if (!switchAnimator.GetCurrentAnimatorStateInfo(0).IsName("On"))
                            switchAnimator.SetTrigger("On");        
                    }
                    else
                    {
                        chaseAnimator.SetTrigger("Hide");
                    }
                    
                    break;
                case PlayerState.Hunter:
                    
                    goalAnimator.SetTrigger("Off");
                    chaseAnimator.SetTrigger("On");
                    switchAnimator.SetTrigger("Off");
                    break;
            }
        }


        void UpdatePlayingState()
        {
            UpdateGoalTimer();
        }

        void UpdateSwitchText()
        {
            if ((GameMode1.Instance as GameMode1).IsLastStep()) return;

            if (GameManager.Instance.GameState != GameState.Playing) return;
           
            var timeLeft = (GameMode.Instance as GameMode1).GetSwitchTimeLeft();
            string s = PlayerController.Instance.State == PlayerState.Hunter ? string.Format(switchPreyTxt, timeLeft) : string.Format(switchHunterTxt, Mathf.CeilToInt(timeLeft));
            switchField.text = s;

       
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

     
        

        

    }
}