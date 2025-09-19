using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class StaminaPowerUp : Singleton<StaminaPowerUp>
    {
        public delegate void BuffDelegate();
        public static BuffDelegate OnBuff;

        public delegate void DepletedDelegate();
        public static DepletedDelegate OnDepleted;

        float depleteSpeedFactor = .7f;
        float chargeDelayFactor = .7f;
        float chargeSpeedFactor = 1.3f;

        float timer = 60;

        float elapsed = 0;
        public float Left
        {
            get { return Mathf.Max(0f, timer - elapsed); }
        }

        bool loop = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!loop) return;

            elapsed += Time.deltaTime;

            if (elapsed > timer)
            {
                loop = false;
                PlayerController.Instance.ResetStaminaStats();

                OnDepleted?.Invoke();
            }
        }
        
        public void BuffSpeed()
        {
            elapsed = 0;
            loop = true;
            PlayerController.Instance.BuffStaminaStats(depleteSpeedFactor, chargeDelayFactor, chargeSpeedFactor);
            OnBuff?.Invoke();
        }
    }
}