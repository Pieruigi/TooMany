using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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


        [SerializeField]
        TMP_Text preyText;

        [SerializeField]
        TMP_Text hunterText;




        string timeStringFormat = "{0:00}:{1:00}";

        CanvasGroup preyCanvasGroup, hunterCanvasGroup;       

        Color activatedColor = new Color(1, 1, 1, 1);
        Color deactivatedColor = new Color(0.5f, 0.5f, .5f, .25f);

        float deactivatedPositionOffsetX = 75f;
        float deactivatedSize = .4f;

        float deactivatedAlpha = .2f;

        float goalTarget;
        int currentStep;

        CanvasGroup preyMessageCanvasGroup, hunterMessageCanvasGroup;

        string goalFormatString = "{0}/{1}";

        bool preSwitching = false;


        protected override void Awake()
        {
            base.Awake();
            preyCanvasGroup = preyTime.GetComponent<CanvasGroup>();
            hunterCanvasGroup = hunterTime.GetComponent<CanvasGroup>();
            hunterCanvasGroup.alpha = deactivatedAlpha;
            var position = (hunterCanvasGroup.transform as RectTransform).anchoredPosition;
            position.x = deactivatedPositionOffsetX;
            (hunterCanvasGroup.transform as RectTransform).anchoredPosition = position;
            hunterCanvasGroup.transform.localScale = Vector3.one * deactivatedSize;
            // var color = preyTime.color;
            // color.a = deactivatedAlpha;
            // preyTime.color = color;
            hunterTime.text = (GameMode.Instance as GameMode1).GetNextHunterTime().ToString();
            preyTime.text = (GameMode.Instance as GameMode1).GetNextPreyTime().ToString();

            currentStep = (GameMode.Instance as GameMode1).GetCurrentStep() / 2;
            goalTarget = ((GameMode.Instance as GameMode1).GetStepMax() + 1) / 2;

            UpdateGoal(string.Format(goalFormatString, currentStep, goalTarget));

            preyMessageCanvasGroup = preyText.GetComponent<CanvasGroup>();
            hunterMessageCanvasGroup = hunterText.GetComponent<CanvasGroup>();
            preyMessageCanvasGroup.alpha = 0;
            hunterMessageCanvasGroup.alpha = 0;
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

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.I))
                HandleOnHunterTimeIncreased(5f);
#endif

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
            GameMode1.OnHunterTimeIncreased += HandleOnHunterTimeIncreased;
      
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            GameMode1.OnHunterTimeIncreased -= HandleOnHunterTimeIncreased;
        }

        private void HandleOnHunterTimeIncreased(float amount)
        {
            float duration = .2f;
            float strength = 30;
            (hunterCanvasGroup.transform as RectTransform).DOShakePosition(duration, strength, vibrato:30, snapping: true);
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            if (oldState == PlayerState.None) return;

            switch (newState)
            {
                case PlayerState.Prey:
                    // preyCanvasGroup.alpha = 1;
                    // hunterCanvasGroup.alpha = 0;
                    HunterToPrey();
                    ShowPreyMessage().Forget();
                    break;
                case PlayerState.Hunter:
                    // preyCanvasGroup.alpha = 0;
                    // hunterCanvasGroup.alpha = 1;
                    PreyToHunter();
                    ShowHunterMessage().Forget();
                    break;
            }
        }

        protected override void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            base.HandleOnGameStateChanged(oldState, newState);

            switch (newState)
            {
                case GameState.Playing:
                    if (oldState == GameState.Starting)
                        ShowPreyMessage().Forget();
                    break;
                case GameState.Winner:
                    UpdateGoal(string.Format(goalFormatString, goalTarget, goalTarget));
                    float duration = .5f;
                    (preyCanvasGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic);
                    preyCanvasGroup.DOFade(deactivatedAlpha, duration).SetEase(Ease.InOutQuad);
                    break;
            }
        }

        void PreyToHunter()
        {
            preSwitching = false;

            // Prey
            float duration = .5f;
            //float strength = 30;
            var pSeq = DOTween.Sequence();
            pSeq.Append((preyCanvasGroup.transform as RectTransform).DOAnchorPosX(-deactivatedPositionOffsetX, duration).SetEase(Ease.InOutElastic));
            pSeq.Join((preyCanvasGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic));

            float alpha = deactivatedAlpha;
            if ((GameMode.Instance as GameMode1).IsLastStep())
                alpha = 0;

            pSeq.Join(preyCanvasGroup.DOFade(deactivatedAlpha, duration).SetEase(Ease.InOutQuad));
          
            // Hunter
            var hSeq = DOTween.Sequence();
            hSeq.Append((hunterCanvasGroup.transform as RectTransform).DOAnchorPosX(0, duration).SetEase(Ease.InOutElastic));
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOScale(1, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad));
          
        }

        void HunterToPrey()
        {
            preSwitching = false;

            // Prey
            float duration = .2f;
            //float strength = 30f;
            var hSeq = DOTween.Sequence();
            hSeq.Append((hunterCanvasGroup.transform as RectTransform).DOAnchorPosX(deactivatedPositionOffsetX, duration).SetEase(Ease.InOutElastic));
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade((GameMode.Instance as GameMode1).IsLastStep() ? 0f : deactivatedAlpha, duration).SetEase(Ease.InOutQuad));
      
            // Hunter
            var pSeq = DOTween.Sequence();
            pSeq.Append((preyCanvasGroup.transform as RectTransform).DOAnchorPosX(0, duration).SetEase(Ease.InOutElastic));
            pSeq.Join((preyCanvasGroup.transform as RectTransform).DOScale(1, duration).SetEase(Ease.InOutElastic));
            pSeq.Join(preyCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad));
               
        }

        async UniTask ShowPreyMessage()
        {
            float duration = .2f;
            float shakeDuration = .5f;
            float strength = 20;
            preyMessageCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad);
            preyMessageCanvasGroup.transform.DOShakePosition(shakeDuration, strength, vibrato:30, snapping:true);

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            preyMessageCanvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad);
            preyMessageCanvasGroup.transform.DOShakePosition(shakeDuration, strength, vibrato:30, snapping:true);
        }

        async UniTask ShowHunterMessage()
        {
            float duration = .2f;
            float shakeDuration = .5f;
            float strength = 20;
            hunterMessageCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad);
            hunterMessageCanvasGroup.transform.DOShakePosition(shakeDuration, strength, vibrato:30, snapping:true);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            hunterMessageCanvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad);
            hunterMessageCanvasGroup.transform.DOShakePosition(shakeDuration, strength, vibrato:30, snapping:true);
        }

        void UpdateTimer()
        {
            var t = (GameMode.Instance as GameMode1).GetTimeRemaining();

            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);

            if (PlayerController.Instance.State == PlayerState.Prey)
            {
                preyTime.text = Mathf.CeilToInt(t).ToString();// string.Format(timeStringFormat, minutes, seconds);
                hunterTime.text = (GameMode.Instance as GameMode1).GetNextHunterTime().ToString();
            }
            else if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                hunterTime.text = Mathf.CeilToInt(t).ToString(); //string.Format(timeStringFormat, minutes, seconds);
                preyTime.text = (GameMode.Instance as GameMode1).GetNextPreyTime().ToString();
            }

            if (t < 3 && !preSwitching && (PlayerController.Instance.State == PlayerState.Prey || PlayerController.Instance.State == PlayerState.Hunter))
            {
                preSwitching = true;
                float count = 12;
                float size = 2f;
                if (PlayerController.Instance.State == PlayerState.Prey)
                    (preyCanvasGroup.transform as RectTransform).DOScale(size, t / count).SetLoops((int)count, LoopType.Yoyo);
                else if (PlayerController.Instance.State == PlayerState.Hunter)
                    (hunterCanvasGroup.transform as RectTransform).DOScale(size, t / count).SetLoops((int)count, LoopType.Yoyo);
                    
            
            }
        }

    }
}