using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace TMOT
{
    public class OptionsManager : SingletonPersistent<OptionsManager>
    {
        public delegate void OptionsChangedDelegate();
        public static OptionsChangedDelegate OnOptionsChanged;

        public static UnityAction OnResolutionChanged;

        [SerializeField]
        AudioMixer mixer;

        public const string MouseSpeedOptionParam = "MouseSpeed";

        public const int MouseSpeedOptionMin = 0;
        public const int MouseSpeedOptionMax = 20;

        public const int MouseSpeedOptionDefault = 10;

        public const string VolumeOptionParam = "Volume";

        public const int VolumeOptionMin = 0;
        public const int VolumeOptionMax = 100;

        public const int VolumeOptionDefault = 100;

        //public const string ResolutionIdOptionParam = "ResolutionId";

        public float MouseSpeed
        {
            get
            {
                var v = PlayerPrefs.GetInt("MouseSpeed", MouseSpeedOptionDefault);
                return Mathf.Lerp(1f, 10f, (float)v / (float)MouseSpeedOptionMax);
            }
        }

        public float Volume
        {
            get
            {
                var v = PlayerPrefs.GetInt("Volume", VolumeOptionDefault);
                Debug.Log("VVVVVVVVV:" + v);
                return Mathf.Log10((float)v / 100f) * 20f;
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
            UpdateMixerVolume();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateMixerVolume()
        {
            mixer.SetFloat("Volume", Volume);
        }

        public void SaveOptions()
        {
            PlayerPrefs.Save();
            UpdateMixerVolume();
            OnOptionsChanged?.Invoke();
        }



    }
}