using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class SpeedPowerUp : Singleton<SpeedPowerUp>
    {
        float speedBuff = 1.2f;

        float timer = 30;

        float elapsed = 0;

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
                PlayerController.Instance.ResetMaxSpeed();
            }
        }

        public void BuffSpeed()
        {
            elapsed = 0;
            loop = true;
            PlayerController.Instance.BuffMaxSpeed(speedBuff);

        }
    }
}