using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class GameMode7_UI : GameModeUI
    {
         [SerializeField]
        GameObject hunterTimer;

        [SerializeField]
        CountdownPlayer countdownPlayer;

              [SerializeField]
        GameObject switchText;

        [SerializeField]
        AudioSource switchAudioSource;

        TMP_Text hunterTimerField;

        Color activatedColor = new Color(1, 1, 1, 1);
        Color deactivatedColor = new Color(0.5f, 0.5f, .5f, .25f);

        float deactivatedSize = .4f;

        float deactivatedAlpha = .2f;

        CanvasGroup hunterCanvasGroup;

        bool preSwitching = false;

        Vector3 hunterLocalPositionDefault;
        
           Vector3 switchTextOriginalPosition;

        protected override void Awake()
        {
            base.Awake();

            hunterTimerField = hunterTimer.GetComponent<TMP_Text>();
            hunterCanvasGroup = hunterTimerField.GetComponent<CanvasGroup>();

            hunterCanvasGroup.transform.localScale = Vector3.one * deactivatedSize;
            hunterCanvasGroup.alpha = deactivatedAlpha;
            hunterLocalPositionDefault = hunterCanvasGroup.transform.localPosition;


            HandleOnHunterTimeIncreased((GameMode.Instance as GameMode7).HunterTime);
            HandleOnProgressUpdated(0, (GameMode.Instance as GameMode7).Goal);

    switchTextOriginalPosition = (switchText.transform as RectTransform).anchoredPosition;

        }

        void LateUpdate()
        {
            UpdateHunterTimer();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameMode.OnProgressUpdated += HandleOnProgressUpdated;

            GameMode7.OnHunterTimeIncreased += HandleOnHunterTimeIncreased;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
              GameMode7.OnSwitchCooldownStarted += HandleOnSwitchCooldownStarted;
            GameMode7.OnSwitchCooldownCompleted += HandleOnSwitchCooldownCompleted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameMode.OnProgressUpdated -= HandleOnProgressUpdated;

            GameMode7.OnHunterTimeIncreased -= HandleOnHunterTimeIncreased;

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
                  GameMode7.OnSwitchCooldownStarted -= HandleOnSwitchCooldownStarted;
            GameMode7.OnSwitchCooldownCompleted -= HandleOnSwitchCooldownCompleted;
        }

        private void HandleOnSwitchCooldownStarted()
        {
            switchText.transform.DOKill();
            (switchText.transform as RectTransform).anchoredPosition = switchTextOriginalPosition;

            var st = switchText.GetComponent<TMP_Text>();
            var c = Color.grey;
            c.a = 0.5f;
            st.color = c;
                     
        }

        private void HandleOnSwitchCooldownCompleted()
        {
            
            var st = switchText.GetComponent<TMP_Text>();
            var c = Color.white;
            c.a = 1f;
            st.color = c;
            (switchText.transform as RectTransform).DOShakeAnchorPos(.2f, 10, 10).SetLoops(5, LoopType.Yoyo).OnComplete(()=> { (switchText.transform as RectTransform).anchoredPosition = switchTextOriginalPosition; });
            switchAudioSource.Play();
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    HunterToPrey();

                    break;

                case PlayerState.Hunter:
                    PreyToHunter();
                    break;
            }
        }

        private void HandleOnHunterTimeIncreased(float time)
        {
            hunterTimerField.text = time.ToString();
        }

        private void HandleOnProgressUpdated(int progress, int goal)
        {
            UpdateGoal($"{progress}/{goal}");
        }

        void PreyToHunter()
        {
            preSwitching = false;

            // Prey
            float duration = .5f;

            // Hunter
            var hSeq = DOTween.Sequence();
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOScale(1, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad));
            hSeq.Join(hunterCanvasGroup.transform.DOShakePosition(.5f, 30f).SetEase(Ease.InOutElastic));
            hSeq.onComplete += () => { hunterCanvasGroup.transform.localPosition = hunterLocalPositionDefault; };
        }

        void HunterToPrey()
        {
            hunterCanvasGroup.transform.DOKill();
            
            preSwitching = false;

            // Prey
            float duration = .5f;

            // Hunter
            var hSeq = DOTween.Sequence();
            hSeq.Join((hunterCanvasGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(hunterCanvasGroup.DOFade(deactivatedAlpha, duration).SetEase(Ease.InOutQuad));
            hSeq.Join(hunterCanvasGroup.transform.DOShakePosition(.5f, 30f).SetEase(Ease.InOutElastic));
            hSeq.onComplete += () => { hunterCanvasGroup.transform.localPosition = hunterLocalPositionDefault; };

            countdownPlayer.Stop();
        }

        void UpdateHunterTimer()
        {
           
            var t = (GameMode.Instance as GameMode7).HunterTime;

            hunterTimerField.text = Mathf.CeilToInt(t).ToString();

            if (PlayerController.Instance.State == PlayerState.Hunter)
            {
                if (t < 3 && t > 2.9f && !preSwitching)
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
}