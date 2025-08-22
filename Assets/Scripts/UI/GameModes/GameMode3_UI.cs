using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class GameMode3_UI : GameModeUI
    {
        [SerializeField]
        CanvasGroup switchTimeGroup;

        TMP_Text switchTimeText;

       
        GameMode3 gameMode;

        bool preSwitching = false;


        protected override void Awake()
        {
            base.Awake();

            // Hide all canvas groups
            switchTimeGroup.alpha = 1;
            switchTimeText = switchTimeGroup.GetComponent<TMP_Text>();

            gameMode = GameMode.Instance as GameMode3;
        }

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
            switch (newState)
            {
                case PlayerState.Prey:
                case PlayerState.Hunter:
                    preSwitching = false;
                    switchTimeText.transform.DOShakePosition(.5f, 30, vibrato:30, snapping:true);
                    break;
            }
        }

        void UpdateTimer()
        {
            var t = gameMode.GetTimeLeft();

            switchTimeText.text = Mathf.CeilToInt(t).ToString();

            if (t < 3 && !preSwitching && (PlayerController.Instance.State == PlayerState.Prey || PlayerController.Instance.State == PlayerState.Hunter))
            {
                preSwitching = true;
                float count = 12;
                float size = 2f;
                
                (switchTimeGroup.transform as RectTransform).DOScale(size, t / count).SetLoops((int)count, LoopType.Yoyo);
                    
            
            }
        }
    }
}