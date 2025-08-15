using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class ModePanel : MonoBehaviour
    {
        

        [SerializeField]
        List<Toggle> modes;

       
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
            // Reset toggle all
            ResetAllToggles();

            // Set current mode
            var mode = (int)GameManager.Instance.GameMode;
            modes[mode].isOn = true;

            // Set listeners
            foreach (var m in modes)
                m.onValueChanged.AddListener((v) => { HandleOnValueChanged(m, v); });

        }

     

        void OnDisable()
        {
            foreach (var m in modes)
                m.onValueChanged.RemoveAllListeners();
        }

        void ResetAllToggles()
        {
            foreach (var mode in modes)
                mode.isOn = false;
        }

        private void HandleOnValueChanged(Toggle toggle, bool value)
        {
            if (!value) return;
            int index = modes.IndexOf(toggle);
            GameManager.Instance.GameMode = (GameModeType)index;
            // for (int i = 0; i < modes.Count; i++)
            // {
            //     if (modes[i].isOn)
            //     {
            //         GameManager.Instance.GameMode = (GameModeType)i;
            //         return;
            //     }
            // }   
            
        }

    }
}