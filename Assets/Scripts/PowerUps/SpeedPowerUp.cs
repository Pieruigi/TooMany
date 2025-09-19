using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class SpeedPowerUp : Singleton<SpeedPowerUp>
    {

        public delegate void BuffDelegate();
        public static BuffDelegate OnBuff;

        public delegate void DepletedDelegate();
        public static DepletedDelegate OnDepleted;

        float speedBuff = 1.3f;

        float timer = 60;

        float elapsed = 0;
        public float Left
        {
            get{ return Mathf.Max(0f, timer - elapsed); }
        }

        bool loop = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
                BuffSpeed();
#endif

            if (!loop) return;

            elapsed += Time.deltaTime;

            if (elapsed > timer)
            {
                loop = false;
                PlayerController.Instance.ResetMaxSpeed();

                OnDepleted?.Invoke();
            }
        }

        public void BuffSpeed()
        {
            elapsed = 0;
            loop = true;
            PlayerController.Instance.BuffMaxSpeed(speedBuff);
            OnBuff?.Invoke();
        }
    }
}