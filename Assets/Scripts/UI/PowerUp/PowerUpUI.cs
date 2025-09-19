using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public abstract class PowerUpUI : MonoBehaviour
    {
        [SerializeField]
        Sprite icon;

        [SerializeField]
        TMP_Text textField;


        protected virtual void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void SetText(string text)
        {
            textField.text = text;
        }
    }
}