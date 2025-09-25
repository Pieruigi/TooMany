using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class ModePanel : MonoBehaviour
    {
        

        [SerializeField]
        List<Toggle> modes;

        [SerializeField]
        GameModeRulePanel rules;

       
        // Start is called before the first frame update
        void Start()
        {
            for(int i=0; i<modes.Count; i++)
                modes[i].interactable = i == 0 || i <= SaveManager.Instance.GameProgress;
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
            rules.ShowRule(mode);


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
#if DEMO
            if (modes.IndexOf(toggle) > 0)
            {
                modes[0].isOn = true;
                DemoManager.Instance.Show(2).Forget();
            }
                
#else
            if (!value) return;
            int index = modes.IndexOf(toggle);
            GameManager.Instance.GameMode = (GameModeType)index;
            rules.ShowRule(index);
#endif            
        }

    }
}