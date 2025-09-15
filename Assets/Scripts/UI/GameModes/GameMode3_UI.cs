using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace TMOT.UI
{
    public class GameMode3_UI : GameModeUI
    {
        [SerializeField]
        CanvasGroup switchTimeGroup;

        [SerializeField]
        CountdownPlayer countdownPlayer;


        TMP_Text switchTimeText;

        float deactivatedSize = .4f;

        float deactivatedAlpha = .2f;


        GameMode3 gameMode;

        int lastCountdownIndex = -1;

        Vector3 hunterLocalPositionDefault;

        bool timeFx = false;


        protected override void Awake()
        {
            base.Awake();

            // Hide all canvas groups
            switchTimeGroup.alpha = 1;
            switchTimeText = switchTimeGroup.GetComponent<TMP_Text>();

            gameMode = GameMode.Instance as GameMode3;


            switchTimeGroup.transform.localScale = Vector3.one * deactivatedSize;
            switchTimeGroup.alpha = deactivatedAlpha;
            hunterLocalPositionDefault = switchTimeGroup.transform.localPosition;

            switchTimeText.text = Mathf.CeilToInt((GameMode.Instance as GameMode3).HunterTime).ToString();

            HandleOnProgressUpdated(0, (GameMode.Instance as GameMode3).Goal);
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
            GameMode3.OnExtraTimeOnKillIncreased += HandleOnExtraTimeOnKill;
            GameMode3.OnHunterTimerIncreased += HandleOnHunterTimeIncreased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            GameMode3.OnExtraTimeOnKillIncreased -= HandleOnExtraTimeOnKill;
            GameMode3.OnHunterTimerIncreased -= HandleOnHunterTimeIncreased;
        }

        private void HandleOnProgressUpdated(int progress, int goal)
        {
            UpdateGoal($"{progress}/{goal}");
        }

        private void HandleOnExtraTimeOnKill(float amount)
        {
            lastCountdownIndex = -1;
        }

        private void HandleOnHunterTimeIncreased(float time)
        {
            switchTimeText.text = time.ToString();
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    lastCountdownIndex = -1;
                    timeFx = false;
                    HunterToPrey();
                    break;
                case PlayerState.Hunter:
                    lastCountdownIndex = -1;
                    timeFx = false;
                    //switchTimeText.transform.DOShakePosition(.5f, 30, vibrato: 30, snapping: true);
                    PreyToHunter();
                    break;
            }
        }


        void PreyToHunter()
        {
            timeFx = false;

            // Prey
            float duration = .5f;

            // Hunter
            var hSeq = DOTween.Sequence();
            hSeq.Join((switchTimeGroup.transform as RectTransform).DOScale(1, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(switchTimeGroup.DOFade(1, duration).SetEase(Ease.InOutQuad));
            hSeq.Join(switchTimeGroup.transform.DOShakePosition(.5f, 30f).SetEase(Ease.InOutElastic));
            hSeq.onComplete += () => { switchTimeGroup.transform.localPosition = hunterLocalPositionDefault; };
        }

        void HunterToPrey()
        {
            switchTimeGroup.transform.DOKill();

            timeFx = false;

            // Prey
            float duration = .5f;

            // Hunter
            var hSeq = DOTween.Sequence();
            hSeq.Join((switchTimeGroup.transform as RectTransform).DOScale(deactivatedSize, duration).SetEase(Ease.InOutElastic));
            hSeq.Join(switchTimeGroup.DOFade(deactivatedAlpha, duration).SetEase(Ease.InOutQuad));
            hSeq.Join(switchTimeGroup.transform.DOShakePosition(.5f, 30f).SetEase(Ease.InOutElastic));
            hSeq.onComplete += () => { switchTimeGroup.transform.localPosition = hunterLocalPositionDefault; };

            countdownPlayer.Stop();
            
            switchTimeText.text = Mathf.CeilToInt((GameMode.Instance as GameMode3).HunterTime).ToString();
        }

        void UpdateTimer()
        {
            if (PlayerController.Instance.State != PlayerState.Hunter) return;

            var t = gameMode.GetTimeLeft();

            switchTimeText.text = Mathf.CeilToInt(t).ToString();

            if (t < 3 && (PlayerController.Instance.State == PlayerState.Prey || PlayerController.Instance.State == PlayerState.Hunter))
            {
                if (Mathf.CeilToInt(t) - t > .1f) return;

                
                float count = 12;
                float size = 2f;

                if (!timeFx)
                {
                    timeFx = true;
                    (switchTimeGroup.transform as RectTransform).DOScale(size, t / count).SetLoops((int)count, LoopType.Yoyo);
                }
                

                //countdownPlayer.Play(2 - Mathf.FloorToInt(t)).Forget();
                int index = 2 - Mathf.FloorToInt(t);
                if (lastCountdownIndex != index)
                {
                    lastCountdownIndex = index;
                    countdownPlayer.Play(index);
                }

            }
           
           
        }

        
    }
}