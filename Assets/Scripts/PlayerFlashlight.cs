using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class PlayerFlashlight : MonoBehaviour
    {
        [SerializeField]
        Color preyColor;

        [SerializeField]
        Color hunterColor;

        [SerializeField]
        Light _light;

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
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    _light.color = preyColor;
                    break;
                case PlayerState.Hunter:
                    _light.color = hunterColor;
                    break;

            }
        }
    }
}