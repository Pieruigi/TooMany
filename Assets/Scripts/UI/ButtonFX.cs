using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMOT.UI
{
    public class ButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        

        
        public void OnPointerEnter(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayEnter();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayClick();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayExit();
        }
    }
}