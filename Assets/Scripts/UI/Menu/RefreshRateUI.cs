using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class RefreshRateUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown dropdown;

        
        List<RefreshRate> refreshRateList = new List<RefreshRate>();




        void Awake()
        {

            // Fill options
            FillOptions();

            SetCurrentOption();

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
            dropdown.onValueChanged.AddListener(HandleOnValueChanged);
            OptionsManager.OnResolutionChanged += HandleOnResolutionChanged;
            
        }



        void OnDisable()
        {
            dropdown.onValueChanged.RemoveAllListeners(); 
            OptionsManager.OnResolutionChanged -= HandleOnResolutionChanged;
        }

        private void HandleOnResolutionChanged()
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            StartCoroutine(UpdateRefreshRate());
            
        }

        private void HandleOnValueChanged(int index)
        {
            // Apply resolution
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode, refreshRateList[index]);

        }

        void SetCurrentOption()
        {
            int index = refreshRateList.IndexOf(Screen.currentResolution.refreshRateRatio);

            dropdown.value = index;
        }

        IEnumerator UpdateRefreshRate()
        {
            yield return null;

            FillOptions();

            SetCurrentOption();
        }

        void FillOptions()
        {
            dropdown.options.Clear();
            refreshRateList.Clear();

            Debug.Log($"TEST - Current Resolution:{Screen.currentResolution}");
            foreach (Resolution r in Screen.resolutions)
            {
                Debug.Log("TEST - resolution:" + r);
            }

            // Get all refresh rate for the current resolution

            var l = Screen.resolutions.Where(r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);

            List<string> options = new List<string>();
            foreach (var r in l)
            {
                refreshRateList.Add(r.refreshRateRatio);
                options.Add(r.refreshRateRatio.ToString());
            }
                

            dropdown.AddOptions(options);
        }
    }

}
