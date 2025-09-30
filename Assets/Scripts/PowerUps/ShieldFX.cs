using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VLB;

namespace TMOT
{
    public class ShieldFX : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup canvasGroup;

        [SerializeField]
        AudioSource activationSource;

        [SerializeField]
        AudioSource deactivationSource;

        float minAlpha = .04f;
        float maxAlpha = .07f;

        float duration = .1f;

        float interval = .02f;

        void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public void Activate()
        {
            // Kill every existing tween
            canvasGroup.DOKill();

            var s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(.8f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(minAlpha, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(.2f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(minAlpha, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(.08f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(minAlpha, duration).SetEase(Ease.OutQuad));
            s.OnComplete(()=>Pulse(1));


            activationSource.Play();

        }

        public void Deactivate()
        {


            canvasGroup.DOKill();

            var s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(.8f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(minAlpha, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(.2f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(minAlpha, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(.08f, duration).SetEase(Ease.OutQuad));
            s.Append(canvasGroup.DOFade(0, duration).SetEase(Ease.OutQuad));


            deactivationSource.Play();
            //canvasGroup.DOFade(.7f, .1f).SetEase(Ease.InQuad).OnComplete(()=> { canvasGroup.DOFade(0f, .1f).SetEase(Ease.InQuad); });

        }

        void Pulse(int dir)
        {
            // Dir is only used the first time as multiplier to enphasize the shield activation with a strong fade in
            float target = dir > 0 ? maxAlpha : minAlpha;
           
            canvasGroup.DOFade(Random.Range(target * .9f, target * 1.1f), Random.Range(duration * .9f, duration * 1.1f)).SetEase(Ease.OutQuad).SetDelay(Random.Range(interval*.9f, interval*1.1f)).OnComplete(()=> { Pulse(-dir); });
        }


    }
}