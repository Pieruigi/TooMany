using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public abstract class PowerUpUI : MonoBehaviour
    {
        [SerializeField]
        Image icon;

        [SerializeField]
        TMP_Text textField;

        [SerializeField]
        Transform root;


        protected virtual void Awake()
        {
            transform.localScale = Vector3.zero;
        }


        protected void SetText(string text)
        {
            textField.text = text;
        }

        public void PopOut()
        {
            transform.DOScale(0, .5f).SetEase(Ease.InElastic).OnComplete(() => { transform.localScale = Vector3.zero; });
        }

        public void PopUp()
        {
            transform.DOScale(1, .5f).SetEase(Ease.OutElastic).OnComplete(() => { transform.localScale = Vector3.one; });
        }

        public void Shake()
        {
            transform.DOShakeScale(10, 50).OnComplete(() => { transform.localScale = Vector3.one; });
        }
    }
}