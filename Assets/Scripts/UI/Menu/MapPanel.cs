using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class MapPanel : MonoBehaviour
    {
        [SerializeField]
        List<Toggle> maps;

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
            ResetAllToggles();

            // Set current mode
            var map = (int)GameManager.Instance.MapId;
            maps[map].isOn = true;

            // Set listeners
            foreach (var m in maps)
                m.onValueChanged.AddListener((v) => { HandleOnValueChanged(m, v); });
        }

         void OnDisable()
        {
            foreach (var m in maps)
                m.onValueChanged.RemoveAllListeners();
        }

        void ResetAllToggles()
        {
            foreach (var map in maps)
                map.isOn = false;
        }

        private void HandleOnValueChanged(Toggle toggle, bool value)
        {
#if DEMO
            if (maps.IndexOf(toggle) > 0)
            {
                maps[0].isOn = true;
                DemoManager.Instance.Show(2).Forget();
            }

#else

            if (!value) return;
            int index = maps.IndexOf(toggle);
            GameManager.Instance.MapId = index;
#endif
     
            
        }
    }
}