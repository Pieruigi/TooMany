using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        [SerializeField]
        Image bar;

        void Awake()
        {
            panel.SetActive(false);
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
            GameManager.OnSceneLoadStarted += HandleOnSceneLoadingStarted;
            GameManager.OnSceneLoadCompleted += HandleOnSceneLoadingCompleted;
            GameManager.OnSceneLoadingProgress += HandleOnSceneLoadProgress;
        }

        void OnDisable()
        {
            GameManager.OnSceneLoadStarted -= HandleOnSceneLoadingStarted;
            GameManager.OnSceneLoadCompleted -= HandleOnSceneLoadingCompleted;
            GameManager.OnSceneLoadingProgress -= HandleOnSceneLoadProgress;
        }

        private void HandleOnSceneLoadProgress(float progress)
        {
            bar.fillAmount = progress;
        }

        private void HandleOnSceneLoadingCompleted()
        {
            panel.SetActive(false);
        }

        private void HandleOnSceneLoadingStarted(int sceneBuildIndex)
        {
            panel.SetActive(true);
        }
    }
}