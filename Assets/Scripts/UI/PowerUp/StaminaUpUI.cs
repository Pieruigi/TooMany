using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT.UI
{
    public class StaminaUpUI : PowerUpUI
    {

        protected override void Awake()
        {
            base.Awake();

            SetText(Mathf.Ceil(StaminaPowerUp.Instance.Left).ToString());
        }

        void Update()
        {
            SetText(Mathf.Ceil(StaminaPowerUp.Instance.Left).ToString());
        }
    }
}