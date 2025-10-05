using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class GameMode4_UI : GameModeUI
    {
        [SerializeField]
        GameObject hunterTimer;

        [SerializeField]
        CountdownPlayer countdownPlayer;

        [SerializeField]
        GameObject switchText;

        [SerializeField]
        AudioSource switchAudioSource;

        [SerializeField]
        AudioSource switchNotReadyAudioSource;

        TMP_Text hunterTimerField;

        Color activatedColor = new Color(1, 1, 1, 1);
        Color deactivatedColor = new Color(0.5f, 0.5f, .5f, .25f);

        float deactivatedSize = .4f;

        float deactivatedAlpha = .2f;

        CanvasGroup hunterCanvasGroup;

        bool preSwitching = false;

        Vector3 hunterLocalPositionDefault;

        Vector3 switchTextOriginalPosition;

        bool notReadySoundDisabled = false;

        protected override void Awake()
        {
            base.Awake();

            hunterTimerField = hunterTimer.GetComponent<TMP_Text>();
            hunterCanvasGroup = hunterTimerField.GetComponent<CanvasGroup>();

            hunterCanvasGroup.transform.localScale = Vector3.one * deactivatedSize;
            hunterCanvasGroup.alpha = deactivatedAlpha;
            hunterLocalPositionDefault = hunterCanvasGroup.transform.localPosition;


            HandleOnHunterTimeIncreased((GameMode.Instance as GameMode4).HunterTime);


            switchTextOriginalPosition = (switchText.transform as RectTransform).anchoredPosition;

        }

        protected override void Start()
        {
            base.Start();
            HandleOnProgressUpdated(0, (GameMode.Instance as GameMode4).Goal);
        }

        protected override void Update()
        {
            base.Update();
       
            if(PlayerController.Instance.State == PlayerState.Prey)
            {
                if (notReadySoundDisabled)
                {
                    notReadySoundDisabled = false;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E) && (GameMode.Instance as GameMode4).SwitchCooldownTimer > 0)
                    {
                        switchNotReadyAudioSource.Play();  
                        switchText.transform.DOKill();  
                        (switchText.transform as RectTransform).DOShakeAnchorPos(.5f, 20, 10).OnComplete(() => { (switchText.transform as RectTransform).anchoredPosition = switchTextOriginalPosition; });
                    }
                        
                }
                
            }
        }

        void LateUpdate()
        {
            

            UpdateHunterTimer();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameMode.OnProgressUpdated += HandleOnProgressUpdated;

            GameMode4.OnHunterTimeIncreased += HandleOnHunterTimeIncreased;
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
            GameMode4.OnSwitchCooldownStarted += HandleOnSwitchCooldownStarted;
            GameMode4.OnSwitchCooldownCompleted += HandleOnSwitchCooldownCompleted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameMode.OnProgressUpdated -= HandleOnProgressUpdated;

            GameMode4.OnHunterTimeIncreased -= HandleOnHunterTimeIncreased;

            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            
            GameMode4.OnSwitchCooldownStarted -= HandleOnSwitchCooldownStarted;
            GameMode4.OnSwitchCooldownCompleted -= HandleOnSwitchCooldownCompleted;
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
            switchText.transform.DOKill();
            var st = switchText.GetComponent<TMP_Text>();
            var c = Color.white;
            c.a = 1f;
            st.color = c;
            (switchText.transform as RectTransform).DOShakeAnchorPos(.5f, 10, 10).OnComplete(()=> { (switchText.transform as RectTransform).anchoredPosition = switchTextOriginalPosition; });
            switchAudioSource.Play();
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    HunterToPrey();
                    notReadySoundDisabled = true;
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
           
            var t = (GameMode.Instance as GameMode4).HunterTime;

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