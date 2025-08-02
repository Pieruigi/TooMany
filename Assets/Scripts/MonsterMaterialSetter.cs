using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{

    public class MonsterMaterialSetter : MonoBehaviour
    {
        [SerializeField]
        Material redMaterial;

        [SerializeField]
        Material greenMaterial;

        [SerializeField]
        Renderer _renderer;

        [SerializeField]
        int materialIndex;

        MonsterController mc;

        DynamicEmissionRange emissionRange;

        void Awake()
        {
            mc = GetComponentInParent<MonsterController>();
            emissionRange = GetComponent<DynamicEmissionRange>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
            SetEmissiveMaterial(GameMode.Instance.StartInHuntingMode ? PlayerState.Hunter : PlayerState.Prey);
        }

        // Update is called once per frame
        void Update()
        {

        }

        // void OnEnable()
        // {
        //     PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        // }

        // void OnDisable()
        // {
        //     PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        // }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            SetEmissiveMaterial(newState);
        }

        void SetEmissiveMaterial(PlayerState playerState)
        {
            Material mat;
            if (playerState == PlayerState.Prey)
                mat = !mc.InvertedBehaviour ? redMaterial : greenMaterial;
            else
                mat = !mc.InvertedBehaviour ? greenMaterial : redMaterial;

            //mat = greenMaterial;
            var mats = _renderer.materials;
            mats[materialIndex] = mat;
            _renderer.materials = mats;

            emissionRange?.Init();
        }
    }
}