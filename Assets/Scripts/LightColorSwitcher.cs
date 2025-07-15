using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class LightColorSwitcher : MonoBehaviour
    {
        [SerializeField]
        Material redMaterial;

        [SerializeField]
        Material greenMaterial;

        [SerializeField]
        Renderer _renderer;

        [SerializeField]
        int materialIndex;

        [SerializeField]
        Light _light;

        [SerializeField]
        ParticleSystem volumetric;

        [SerializeField]
        Color redLightColor;

        [SerializeField]
        Color greenLightColor;

        void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            redLightColor = LevelController.Instance.PlayerPreyColor;
            greenLightColor = LevelController.Instance.PlayerHunterColor;
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
            // Set light emission
            SetEmissiveMaterial();

            // Set light
            SetLightColor();

           

        }

        void SetEmissiveMaterial()
        {
            if (!_renderer) return;

            Material mat;
            if (PlayerController.Instance.State == PlayerState.Prey)
                mat = redMaterial;
            else
                mat = greenMaterial;

            //mat = greenMaterial;
            var mats = _renderer.materials;
            mats[materialIndex] = mat;
            _renderer.materials = mats;
        }

        void SetLightColor()
        {
            Color col;
            if (PlayerController.Instance.State == PlayerState.Prey)
                col = redLightColor;
            else
                col = greenLightColor;

            if(_light)
                _light.color = col;

            if (volumetric)
            {
                var main = volumetric.main;
                float alpha = main.startColor.color.a;
                main.startColor = new Color(col.r, col.g, col.b, alpha);
              
            }
                 
        }

        

    }
}