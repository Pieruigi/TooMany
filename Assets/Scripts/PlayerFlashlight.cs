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

        }

        // Update is called once per frame
        void Update()
        {

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
                    StartCoroutine(FlickerThenSwitch(preyColor));
                    break;
                case PlayerState.Hunter:
                    StartCoroutine(FlickerThenSwitch(hunterColor));
                    //_light.color = hunterColor;
                    break;

            }
        }

       
         System.Collections.IEnumerator FlickerThenSwitch(Color toColor)
        {
            if (_light.color == toColor) yield break;

            yield return _light.DOIntensity(0, .180f).SetEase(Ease.Flash).WaitForCompletion();

            // float elapsed = 0f;
            // while (elapsed < flickerDuration)
            // {
            //     float nextIntensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            //     _light.DOIntensity(nextIntensity, flickerSpeed).SetEase(Ease.Flash);
            //     yield return new WaitForSeconds(flickerSpeed);
            //     elapsed += flickerSpeed;
            // }

            // // Fade out to 0 intensity
            // yield return _light.DOIntensity(0f, 0.5f).WaitForCompletion();

            // Change color
            _light.color = toColor;

            // Fade in with new color and flicker back
            // elapsed = 0f;
            // while (elapsed < flickerDuration)
            // {
            //     float nextIntensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            //     _light.DOIntensity(nextIntensity, flickerSpeed).SetEase(Ease.Flash);
            //     yield return new WaitForSeconds(flickerSpeed);
            //     elapsed += flickerSpeed;
            // }

            // Stabilize at final intensity
            _light.DOIntensity(intensity, 0.180f).SetEase(Ease.Flash);
        }
    }
}