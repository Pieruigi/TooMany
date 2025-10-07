using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class VSyncUI : MonoBehaviour
    {
        [SerializeField]
        Toggle toggle;

        void Awake()
        {
            Init();
        }

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
            toggle.onValueChanged.AddListener(HandleOnValueChanged);
        }

        

        void OnDisable()
        {
            toggle.onValueChanged.RemoveAllListeners(); 
        }

        private void HandleOnValueChanged(bool isOn)
        {
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        }

        void Init()
        {
            toggle.isOn = QualitySettings.vSyncCount > 0;
        }

        
    }
}