using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT.UI
{
    public class SpeedUpUI : PowerUpUI
    {

        protected override void Awake()
        {
            base.Awake();

            SetText(Mathf.Ceil(SpeedPowerUp.Instance.Left).ToString());
        }
    }
}