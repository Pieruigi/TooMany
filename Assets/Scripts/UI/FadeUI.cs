using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class FadeUI : Singleton<FadeUI>
    {
        [SerializeField]
        Image panel;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
        }

        void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        private void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Starting:
#pragma warning disable CS4014
                    FadeIn();
#pragma warning restore CS4014
                    break;
            }
        }

        public async Task FadeIn(float duration = .5f)
        {
            await panel.DOColor(new Color(0, 0, 0, 0), duration).AsyncWaitForCompletion();
        }

        public async Task FadeOut(float duration = .5f)
        {
            await panel.DOColor(new Color(0, 0, 0, 1), duration).AsyncWaitForCompletion();
        }
        

    }
}