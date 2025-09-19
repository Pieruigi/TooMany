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
            SpeedPowerUp.OnDepleted += HandleOnSpeedDepleted;
        }

        void OnDisable()
        {
            SpeedPowerUp.OnBuff -= HandleOnSpeedBuff;
            SpeedPowerUp.OnDepleted -= HandleOnSpeedDepleted;
        }

        private void HandleOnSpeedDepleted()
        {
            PopOut(ref speed);
        }

        private void HandleOnSpeedBuff()
        {
            PopUpOrShake(ref speed);
        }

        private void PopOut(ref GameObject obj)
        {
            var tmp = obj;
            obj = null;
            tmp.GetComponent<PowerUpUI>().PopOut();
            Destroy(tmp, 1);
        }

        private void PopUpOrShake(ref GameObject obj)
        {
            if (!obj)
            {
                obj = Instantiate(speedUpPrefab, root);
                obj.GetComponent<PowerUpUI>().PopUp();
            }
            else
            {
                obj.GetComponent<PowerUpUI>().Shake();
            }

        }
    }
}