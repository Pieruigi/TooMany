using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT.UI
{
    public class PowerUpPanelUI : MonoBehaviour
    {
        [SerializeField]
        GameObject speedUpPrefab;

        [SerializeField]
        GameObject staminaUpPrefab;

        [SerializeField]
        GameObject shieldPrefab;



        [SerializeField]
        Transform root;

        GameObject speed;
        GameObject stamina;
        GameObject shield;

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
            StaminaPowerUp.OnBuff += HandleOnStaminaBuff;
            StaminaPowerUp.OnDepleted += HandleOnStaminaDepleted;
            // ShieldPowerUp.OnActivated += HandleOnShieldActivated;
            // ShieldPowerUp.OnDeactivated += HandleOnShieldDeactivated;
        }

        void OnDisable()
        {
            SpeedPowerUp.OnBuff -= HandleOnSpeedBuff;
            SpeedPowerUp.OnDepleted -= HandleOnSpeedDepleted;
            StaminaPowerUp.OnBuff -= HandleOnStaminaBuff;
            StaminaPowerUp.OnDepleted -= HandleOnStaminaDepleted;
            // ShieldPowerUp.OnActivated -= HandleOnShieldActivated;
            // ShieldPowerUp.OnDeactivated -= HandleOnShieldDeactivated;
        }

        private void HandleOnShieldActivated()
        {
            PopUpOrShake(ref shield, shieldPrefab);
        }

        private void HandleOnShieldDeactivated()
        {
            PopOut(ref shield);
        }

        private void HandleOnStaminaDepleted()
        {
            PopOut(ref stamina);
        }

        private void HandleOnStaminaBuff()
        {
            PopUpOrShake(ref stamina, staminaUpPrefab);
        }

        private void HandleOnSpeedDepleted()
        {
            PopOut(ref speed);
        }

        private void HandleOnSpeedBuff()
        {
            PopUpOrShake(ref speed, speedUpPrefab);
        }

        private void PopOut(ref GameObject obj)
        {
            var tmp = obj;
            obj = null;
            tmp.GetComponent<PowerUpUI>().PopOut();
            Destroy(tmp, 1);
        }

        private void PopUpOrShake(ref GameObject obj, GameObject prefab)
        {
            if (!obj)
            {
                obj = Instantiate(prefab, root);
                obj.GetComponent<PowerUpUI>().PopUp();
            }
            else
            {
                obj.GetComponent<PowerUpUI>().Shake();
            }

        }
    }
}