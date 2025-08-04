using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class PlayerEmission : MonoBehaviour
    {
        [SerializeField]
        Renderer _renderer;

        [SerializeField]
        int materialIndex;

        [SerializeField]
        Material preyMaterial;

        [SerializeField]
        Material hunterMaterial;

        // Start is called before the first frame update
        void Start()
        {
            SetMaterial(GameMode.Instance.StartInHuntingMode ? hunterMaterial : preyMaterial);
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    SetMaterial(preyMaterial);
                    break;
                case PlayerState.Hunter:
                    SetMaterial(hunterMaterial);
                    break;
            }
            
            
        }

        void SetMaterial(Material mat)
        {
            var mats = _renderer.materials;
            mats[materialIndex] = mat;
            _renderer.materials = mats;
        }
    }
}