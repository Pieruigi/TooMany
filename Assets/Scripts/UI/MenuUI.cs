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

        void Awake()
        {
            HidePanelAll();

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
            foreach (var panel in panels)
                HidePanel(panel);
        }

        private void HidePanel(CanvasGroup panel)
        {
            panel.DOFade(0, fadeTime).SetEase(Ease.InOutQuad);
        }

        void ShowPanel(CanvasGroup panel)
        {
            panel.DOFade(1, fadeTime).SetEase(Ease.InOutQuad);
        }

        private void ShowPanel(int index)
        {
            ShowPanel(panels[index]);
        }
    }
}