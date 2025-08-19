using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class UnlockImage : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        Button button;

        Tweener shakeTween;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            button = GetComponentInParent<Button>();
            button.onClick.AddListener(() => { SaveManager.Instance.ResetNewGameModeUnlocked(); canvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad).onComplete += ()=> { shakeTween?.Kill(); }; });
        }

        // Start is called before the first frame update
        void Start()
        {
            var hidden = !SaveManager.Instance.IsNewGameModeUnlocked();

            if (hidden)
                canvasGroup.alpha = 0;
            else
                StartTween();
        }

        // Update is called once per frame
        void Update()
        {

        }



        void StartTween()
        {
            shakeTween = (transform as RectTransform).DOShakePosition(duration: 1f, strength: 10, vibrato: 40, snapping: true, randomness: 90).SetLoops(-1);

            
        }

       
    }
}