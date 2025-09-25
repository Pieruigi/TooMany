using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        

        string msgFormatStr = "Experiment speed: x{0}";

        bool loop = false;

        float timer = 0;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!loop) return;

            timer -= Time.unscaledDeltaTime;
            if (timer < 1)
            {
                startField.text = "GO!!!";
            }
            else if (timer < 2)
            {
                startField.text = "Ready.";
            }
            else if (timer < 3)
            {
                startField.text = ""; //"...";
            }
            else if (timer < 4)
            {
                startField.text = ""; //"..";
            }
               else if (timer < 5)
            {
                startField.text = ".";
            }
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
                    msgField.text = string.Format(msgFormatStr, GameManager.Instance.GameSpeed.ToString(CultureInfo.InvariantCulture));
                    loop = true;
                    timer = GameManager.StartingDelay;
                    break;
                case GameState.Playing:
                    loop = false;
                    transform.GetChild(0).gameObject.SetActive(false);   
                    break;
            }
            
        }
    }
}