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

        void Awake()
        {
            mc = GetComponentInParent<MonsterController>();
            
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
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            SetEmissiveMaterial();
        }

        void SetEmissiveMaterial()
        {
            Material mat;
            if (PlayerController.Instance.State == PlayerState.Prey)
                mat = !mc.InvertedBehaviour ? redMaterial : greenMaterial;
            else
                mat = !mc.InvertedBehaviour ? greenMaterial : redMaterial;

            //mat = greenMaterial;
            var mats = _renderer.materials;
            mats[materialIndex] = mat;
            _renderer.materials = mats;
        }
    }
}