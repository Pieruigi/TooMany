using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class ResolutionUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Dropdown dropdown;

        string resolutionFormatString = "{0}x{1}";

        
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
            // Get resolution width and height
            string[] splits = dropdown.options[index].text.ToLower().Split("x");
            int w = int.Parse(splits[0]);
            int h = int.Parse(splits[1]);

            // Find the higher refresh rate for that resolution (should be the last, so we get the last)
            var res = Screen.resolutions.ToList().OrderByDescending(r => r.refreshRateRatio).ToList().FirstOrDefault(r => r.width == w && r.height == h);

            // Apply resolution
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, res.refreshRateRatio);

            OptionsManager.OnResolutionChanged?.Invoke();
        }

        void SetCurrentOption()
        {
            string s = string.Format(resolutionFormatString, Screen.currentResolution.width, Screen.currentResolution.height);
            int index = dropdown.options.FindIndex(o => o.text.Equals(s));

            dropdown.value = index;
        }

        void FillOptions()
        {
            dropdown.ClearOptions();

            var tmp = Screen.resolutions.ToList();
            List<string> options = new List<string>();

            foreach (var res in tmp)
            {
                int w = res.width;
                int h = res.height;
                string s = string.Format(resolutionFormatString, w, h);

                if (!options.Exists(d => d.Equals(s)))
                    options.Add(s);

            }

            dropdown.AddOptions(options);
        }
        
        
    }
}