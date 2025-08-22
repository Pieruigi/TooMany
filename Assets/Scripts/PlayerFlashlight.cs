using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class PlayerFlashlight : MonoBehaviour
    {
        [SerializeField]
        Color preyColor;

        [SerializeField]
        Color hunterColor;

        [SerializeField]
        Light _light;

         float flickerDuration = .5f;
    float flickerSpeed = 1f;
    float minIntensity = 0.2f;
        float maxIntensity = 1.5f;

        float intensity;


        void Awake()
        {
            intensity = _light.intensity;
            minIntensity = .2f;
            maxIntensity = intensity * 1.5f;
            _light.color = preyColor;
        }

        // Start is called before the first frame update
        void Start()
        {
            // preyColor = LevelController.Instance.PlayerPreyColor;
            // hunterColor = LevelController.Instance.PlayerHunterColor;

            _light.color = GameMode.Instance.StartInHuntingMode ? hunterColor : preyColor;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
                _light.DOIntensity(0, 0.05f).SetEase(Ease.Flash).SetLoops(6, LoopType.Yoyo);
#endif
        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    //_light.color = preyColor;
                    //StartCoroutine(FlickerThenSwitch(preyColor));
                    _light.DOIntensity(0, 0.05f).SetEase(Ease.Flash).SetLoops(6, LoopType.Yoyo);
                    break;
                case PlayerState.Hunter:
                    //StartCoroutine(FlickerThenSwitch(hunterColor));
                    _light.DOIntensity(0, 0.05f).SetEase(Ease.Flash).SetLoops(6, LoopType.Yoyo);
                    break;

            }
        }


        System.Collections.IEnumerator FlickerThenSwitch(Color toColor)
        {
            if (_light.color == toColor) yield break;

            _light.DOIntensity(0, 0.1f).SetEase(Ease.Flash).SetLoops(4, LoopType.Yoyo);

            //yield return _light.DOIntensity(0, .180f).SetEase(Ease.Flash).WaitForCompletion();

            // Change color
            // _light.color = toColor;

            // // Stabilize at final intensity
            // _light.DOIntensity(intensity, 0.180f).SetEase(Ease.Flash);
        }
    }
}