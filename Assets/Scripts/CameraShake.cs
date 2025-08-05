using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using RetroShadersPro.URP;
using TMOT.UI;
using UnityEngine.UI;


namespace TMOT
{
    public class CameraShake : Singleton<CameraShake>
    {
        [SerializeField]
        Image brokenGlassImage;

        [SerializeField]
        GameObject playerArms;

        private Vector3 originalPos;
        private Tween currentShake;



        protected override void Awake()
        {
            base.Awake();

            originalPos = transform.localPosition;

            brokenGlassImage.enabled = false;
        }

        public void Shake(float duration = 0.2f, float strength = 0.3f, int vibrato = 10, float randomness = 90f)
        {


            // Se c'è già uno shake in corso, fermalo
            currentShake?.Kill();

            // Reset posizione per evitare drift
            transform.localPosition = originalPos;

            // Avvia un nuovo shake
            currentShake = transform.DOShakePosition(duration, strength, vibrato, randomness)
                                    .OnComplete(() => { transform.localPosition = originalPos; });
        }

        public async void Die()
        {
            currentShake?.Kill();

            transform.localPosition = originalPos;


            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOLocalMoveY(.5f, 1f, false).SetEase(Ease.InOutElastic));
            seq.Join(transform.DOLocalRotate(new Vector3(0f, 0f, 90f), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutElastic));

            await Task.Delay(TimeSpan.FromSeconds(.8f));

            await FadeUI.Instance.FadeOut(.05f);
            // Activate vhs distortion

            var volume = FindObjectOfType<Volume>();
            if (volume.profile.TryGet<CRTSettings>(out var crt))
            {
                crt.randomWear.overrideState = true;
                crt.aberrationStrength.overrideState = true;
                crt.trackingTexture.overrideState = true;
                crt.trackingSize.overrideState = true;
                crt.trackingStrength.overrideState = true;
                crt.trackingSpeed.overrideState = true;
                crt.trackingJitter.overrideState = true;
                crt.trackingColorDamage.overrideState = true;
                crt.trackingLinesThreshold.overrideState = true;
                crt.trackingLinesColor.overrideState = true;
            }

            brokenGlassImage.enabled = true;

            // Hide player arms
            playerArms.SetActive(false);

            await Task.Delay(TimeSpan.FromSeconds(.1f));
            
#pragma warning disable CS4014
            FadeUI.Instance.FadeIn(.05f);
#pragma warning restore CS4014
        }
    }
}