using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class OptionsPanel : SingletonPersistent<OptionsPanel>
    {


        [SerializeField]
        Slider mouseSpeedSlider;

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            mouseSpeedSlider.onValueChanged.AddListener(HandleOnMouseSpeedChanged);

            // Init
            var v = PlayerPrefs.GetInt(OptionsManager.MouseSpeedOptionParam, OptionsManager.MouseSpeedOptionDefault);
            mouseSpeedSlider.value = v;
        }

        void OnDisable()
        {
            mouseSpeedSlider.onValueChanged.RemoveAllListeners();
        }

        private void HandleOnMouseSpeedChanged(float value)
        {
            PlayerPrefs.SetInt(OptionsManager.MouseSpeedOptionParam, (int)value);
            
            OptionsManager.Instance.SaveOptions();
        }
    }
}