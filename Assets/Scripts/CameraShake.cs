using UnityEngine;
using DG.Tweening;
using System;


namespace TMOT
{
    public class CameraShake : Singleton<CameraShake>
    {


        private Vector3 originalPos;
        private Tween currentShake;

       

        protected override void Awake()
        {
            base.Awake();

            originalPos = transform.localPosition;
        }

        public void Shake(float duration = 0.2f, float strength = 0.3f, int vibrato = 10, float randomness = 90f)
        {
           

            // Se c'è già uno shake in corso, fermalo
            currentShake?.Kill();

            // Reset posizione per evitare drift
            transform.localPosition = originalPos;

            // Avvia un nuovo shake
            currentShake = transform.DOShakePosition(duration, strength, vibrato, randomness)
                                    .OnComplete(() => transform.localPosition = originalPos);
        }
    }
}