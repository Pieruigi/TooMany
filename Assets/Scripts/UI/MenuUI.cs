using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMOT.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField]
        List<CanvasGroup> panels;

        [SerializeField]
        int panelDefault = 0;

        float fadeTime = .25f;

        CanvasGroup current;

        void Awake()
        {
            foreach (var p in panels)
            {
                p.alpha = 0;
                p.blocksRaycasts = false;
            }
                

            if (!(panelDefault < 0))
                    ShowPanel(panelDefault);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        

        void HidePanelAll()
        {
            current = null;
            foreach (var panel in panels)
                HidePanel(panel);
        }

        private void HidePanel(CanvasGroup panel)
        {
            panel.blocksRaycasts = false;
            panel.DOFade(0, fadeTime).SetEase(Ease.InOutQuad);
        }

        public void ShowPanel(CanvasGroup panel)
        {
            if (current) HidePanel(current);
            current = panel;
            panel.blocksRaycasts = true;
            panel.DOFade(1, fadeTime).SetEase(Ease.InOutQuad);
        }

        public void ShowPanel(int index)
        {
            ShowPanel(panels[index]);
        }
    }
}