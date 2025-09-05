using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class PlayerEmission : MonoBehaviour
    {
        // [SerializeField]
        // Renderer _renderer;

        // [SerializeField]
        // int materialIndex;

        // [SerializeField]
        // Material preyMaterial;

        // [SerializeField]
        // Material hunterMaterial;

        [SerializeField]
        Color preyColor, hunterColor;

        [SerializeField]
        float preyColorIntensity, hunterColorIntensity;

        [SerializeField]
        Material material;


        Vector4 _hdrColor;

        float switchDuration = 1;

        // Start is called before the first frame update
        void Start()
        {
            //SetMaterial(GameMode.Instance.StartInHuntingMode ? hunterMaterial : preyMaterial);

            //ForceHdrColor(GameMode.Instance.StartInHuntingMode ? (Vector4)hunterColor * hunterColorIntensity : (Vector4)preyColor * preyColorIntensity);

            //DOTween.To(() => (Vector4)preyColor * preyColorIntensity, x => material.SetVector("_BaseColor", x), (Vector4)preyColor * preyColorIntensity / 4f, switchDuration).SetLoops(-1, LoopType.Yoyo);
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
                    //SwitchHdrColor(preyColor * preyColorIntensity);
                    break;
                case PlayerState.Hunter:
                    //SwitchHdrColor(hunterColor * hunterColorIntensity);
                    break;
            }


        }

        // void SetMaterial(Material mat)
        // {
        //     return;
        //     var mats = _renderer.materials;
        //     mats[materialIndex] = mat;
        //     _renderer.materials = mats;
        // }

        void ForceHdrColor(Vector4 hdrColor)
        {
            material.SetVector("_BaseColor", hdrColor);
        }

        void SwitchHdrColor(Vector4 hdrColor)
        {

            DOTween.To(() => material.GetVector("_BaseColor"), x => material.SetVector("_BaseColor", x), hdrColor, switchDuration).SetEase(Ease.InOutQuad);
        }


    }
}