using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT.UI
{
    public class GameModeRulePanel : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> rules;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void HideAll()
        {
            foreach (var rule in rules)
                rule.SetActive(false);
        }

        public void ShowRule(int index)
        {
            HideAll();
            rules[index].SetActive(true);
        }

    }
}