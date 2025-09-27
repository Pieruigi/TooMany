using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class ShieldPowerUp : Singleton<ShieldPowerUp>
    {
        public delegate void ActivatedDelegate();
        public static ActivatedDelegate OnActivated;

        public delegate void DeactivatedDelegate();
        public static DeactivatedDelegate OnDeactivated;

        [SerializeField]
        GameObject shield;

        int hitMax = 1;

        int hitLeft = 0;

        ShieldFX shieldFx;

        protected override void Awake()
        {
            base.Awake();

            shieldFx = shield.GetComponent<ShieldFX>();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Activate()
        {
            hitLeft = hitMax;


            shieldFx.Activate();
            // Create shield object
            // shield = Instantiate(shieldPrefab, gameObject.transform);
            // shield.transform.localPosition = Vector3.zero;
            // shield.transform.localRotation = Quaternion.identity;


            OnActivated?.Invoke();
        }

        void Deactivate()
        {
            shieldFx.Deactivate();


            OnDeactivated?.Invoke();
        }

        /// <summary>
        /// Returns not deflected damage
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int DeflectDamage(int amount)
        {
            if (hitLeft <= 0) return amount;
            
            var retDamage = Mathf.Max(0, amount - hitLeft);
            hitLeft -= amount;
            if (hitLeft <= 0) Deactivate();

            return retDamage;
        }

        public bool IsActive()
        {
            return hitLeft > 0;
        }
    }
}