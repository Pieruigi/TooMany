using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace TMOT.UI
{
    public class MessageUI : MonoBehaviour
    {
        TMP_Text textField;

        CanvasGroup canvasGroup;

        bool busy = false;

        float fadeTime = .5f;

        void Awake()
        {
            textField = GetComponentInChildren<TMP_Text>();
            canvasGroup = GetComponentInChildren<CanvasGroup>();

            canvasGroup.alpha = 0;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show()
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAA");
            if (busy) return;

            busy = true;
            Sequence seq = DOTween.Sequence();

            seq.Append(canvasGroup.DOFade(1, fadeTime));
            seq.Append(canvasGroup.DOFade(0, fadeTime).SetDelay(1f));
            seq.onComplete += () => { busy = false; };

            UIAudioManager.Instance.PlayFailed();
        }
    }
}