using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class OptionsManager : SingletonPersistent<OptionsManager>
    {
        public delegate void OptionsChangedDelegate();
        public static OptionsChangedDelegate OnOptionsChanged;

        public const string MouseSpeedOptionParam = "MouseSpeed";

        public const int MouseSpeedOptionMin = 0;
        public const int MouseSpeedOptionMax = 20;

        public const int MouseSpeedOptionDefault = 10;
        
        public float MouseSpeed
        {
            get
            {
                var v = PlayerPrefs.GetInt("MouseSpeed", MouseSpeedOptionDefault);
                Debug.Log($"TEST - MouseSpeed:{v}");
                return Mathf.Lerp(1f, 10f, (float)v / (float)MouseSpeedOptionMax);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // Load options
            
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SaveOptions()
        {
            PlayerPrefs.Save();
            OnOptionsChanged?.Invoke();
        }
    }
}