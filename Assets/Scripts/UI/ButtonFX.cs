using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class ButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {

        Vector3 originalPosition;


        void Awake()
        {
            originalPosition = (transform as RectTransform).anchoredPosition;
           
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            (transform as RectTransform).DOShakeAnchorPos(.5f, 10, 10).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            UIAudioManager.Instance.PlayEnter();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayClick();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            (transform as RectTransform).DOKill();
            (transform as RectTransform).anchoredPosition = originalPosition;
            UIAudioManager.Instance.PlayExit();
        }
    }
}