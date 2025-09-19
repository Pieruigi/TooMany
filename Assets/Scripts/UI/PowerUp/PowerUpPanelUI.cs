using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT.UI
{
    public class PowerUpPanelUI : MonoBehaviour
    {
        [SerializeField]
        GameObject speedUpPrefab;

        [SerializeField]
        Transform root;

        GameObject speed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            SpeedPowerUp.OnBuff += HandleOnSpeedBuff;
            
        }

        void OnDisable()
        {
            SpeedPowerUp.OnBuff -= HandleOnSpeedBuff;
        }


        private void HandleOnSpeedBuff()
        {
            if (!speed)
                speed = Instantiate(speedUpPrefab, root);
        }
    }
}