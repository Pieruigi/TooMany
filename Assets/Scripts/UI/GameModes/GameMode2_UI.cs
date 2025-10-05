using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class GameMode2_UI : GameModeUI
    {
        [SerializeField]
        TMP_Text hunterTime;

        [SerializeField]
        TMP_Text preyText;

        [SerializeField]
        TMP_Text hunterText;

        [SerializeField]
        CountdownPlayer countdownPlayer;

        string goalFormatString = "{0}/{1}";

        float deactivatedSize = .4f;

        float deactivatedAlpha = .2f;

        CanvasGroup hunterCanvasGroup, preyMessageCanvasGroup, hunterMessageCanvasGroup;

        bool preSwitching = false;



        protected override void Awake()
        {
            base.Awake();

            hunterCanvasGroup = hunterTime.GetComponent<CanvasGroup>();
            hunterCanvasGroup.alpha = deactivatedAlpha;
            hunterCanvasGroup.transform.localScale = Vector3.one * deactivatedSize;

            hunterTime.text = (GameMode.Instance as GameMode2).GetNextHunterTime().ToString();

            preyMessageCanvasGroup = preyText.GetComponent<CanvasGroup>();
            hunterMessageCanvasGroup = hunterText.GetComponent<CanvasGroup>();
            preyMessageCanvasGroup.alpha = 0;
            hunterMessageCanvasGroup.alpha = 0;
        }

        protected override void Start()
        {
            base.Start();

            //UpdateGoal($"0/{(GameMode.Instance as GameMode2).Goal}");
        }

        protected override void Update()
        {
            base.Update();

            UpdateHunterTimer();
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
                    HunterToPrey();
                    ShowPlayerStateChangedMessage(preyMessageCanvasGroup).Forget();
                    break;
                case PlayerState.Hunter:
                    PreyToHunter();
                    ShowPlayerStateChangedMessage(hunterMessageCanvasGroup).Forget();
                    break;
            }
        }

     

        void PreyToHunter()
        {
            preSwitching = false;

            float duration = .5f;
            float strength = 30;

            var hSeq = DOTween.Sequence();
            hSeq.Append((hunterCanvasGroup.transform as RectTransform).DOScale(1, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad));
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOShakePosition(duration, strength, vibrato:30, snapping:true));

        }

        void HunterToPrey()
        {
            preSwitching = false;

            float duration = .2f;
            float strength = 30f;
            var hSeq = DOTween.Sequence();
            hSeq.Append((hunterCanvasGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade((GameMode.Instance as GameMode2).IsLastStep() ? 0f : deactivatedAlpha, duration).SetEase(Ease.InOutQuad));
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOShakePosition(duration, strength, vibrato:30, snapping:true));

        }
        
        void UpdateHunterTimer()
        {
            var t = (GameMode.Instance as GameMode2).GetHunterTimeRemaining();

            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            // if (PlayerController.Instance.State == PlayerState.Prey)
            // {
            //     hunterTime.text = (GameMode.Instance as GameMode1).GetNextHunterTime().ToString();
            // }
            // else if (PlayerController.Instance.State == PlayerState.Hunter)
            // {
            //     hunterTime.text = Mathf.CeilToInt(t).ToString(); //string.Format(timeStringFormat, minutes, seconds);
            // }

            hunterTime.text = Mathf.CeilToInt(t).ToString(); //string.Format(timeStringFormat, minutes, seconds);

            if (t < 3 && !preSwitching && PlayerController.Instance.State == PlayerState.Hunter)
            {
                preSwitching = true;
                float count = 12;
                float size = 2f;
                
                (hunterCanvasGroup.transform as RectTransform).DOScale(size, t / count).SetLoops((int)count, LoopType.Yoyo);

                countdownPlayer.Play().Forget();
            }
        }
    }
}