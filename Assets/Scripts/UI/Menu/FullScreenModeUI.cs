using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class FullScreenModeUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown dropdown;

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

        }



        void OnDisable()
        {
            dropdown.onValueChanged.RemoveAllListeners();

        }

        private void HandleOnValueChanged(int index)
        {
            int fullScreenMode = index;
            if (index == 2) index++; // The option 2 in full screen mode enumerator is IOS only, so the windowed mode is on index 3.
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, (FullScreenMode)index);

            OptionsManager.OnResolutionChanged?.Invoke();
        }

        void FillOptions()
        {
            List<string> options = new List<string>();
            options.Add(FullScreenMode.ExclusiveFullScreen.ToString());
            options.Add(FullScreenMode.FullScreenWindow.ToString());
            options.Add(FullScreenMode.Windowed.ToString()); // Value = 3
            dropdown.ClearOptions();
            dropdown.AddOptions(options);

        }

        void SetCurrentOption()
        {
            dropdown.value = dropdown.options.FindIndex(d => d.text.ToLower().Equals(Screen.fullScreenMode.ToString().ToLower()));
        }
    }
}