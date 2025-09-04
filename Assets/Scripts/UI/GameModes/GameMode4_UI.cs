using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class GameMode4_UI : GameModeUI
    {
        [SerializeField]
        GameObject hunterTimer;

        TMP_Text hunterTimerField;

        protected override void Awake()
        {
            base.Awake();

            hunterTimerField = hunterTimer.GetComponent<TMP_Text>();

            HandleOnHunterTimeIncreased((GameMode.Instance as GameMode4).HunterTime);
        }

        void LateUpdate()
        {
            hunterTimerField.text = Mathf.CeilToInt((GameMode.Instance as GameMode4).HunterTime).ToString();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GameMode.OnProgressUpdated += HandleOnProgressUpdated;
            GameMode4.OnHunterTimeIncreased += HandleOnHunterTimeIncreased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameMode.OnProgressUpdated -= HandleOnProgressUpdated;
            GameMode4.OnHunterTimeIncreased -= HandleOnHunterTimeIncreased;
        }

        private void HandleOnHunterTimeIncreased(float time)
        {
            hunterTimerField.text = time.ToString();
        }

        private void HandleOnProgressUpdated(int progress, int goal)
        {
            UpdateGoal($"{progress}/{goal}");
        }
    }
}