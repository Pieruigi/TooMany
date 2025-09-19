using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class StartPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text msgField;

        [SerializeField]
        TMP_Text startField;

        

        string msgFormatStr = "Experiment speed: {0}";


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
            GameManager.OnStateChanged += HandleOnGameStateChanged;

            

        }

        void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        private void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Starting:
                    msgField.text = string.Format(msgFormatStr, GameManager.Instance.GameSpeed);

                    break;
                case GameState.Playing:
                    transform.GetChild(0).gameObject.SetActive(false);   
                    break;
            }
            
        }
    }
}