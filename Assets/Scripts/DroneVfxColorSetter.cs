using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace TMOT
{
    public class DroneVfxColorSetter : MonoBehaviour
    {
        [SerializeField]
        Light _light;

        [SerializeField]
        ParticleSystem volumetric;

        [SerializeField]
        Color redLightColor;

        [SerializeField]
        Color blueLightColor;

        bool isInverted = false;


        // Start is called before the first frame update
        void Start()
        {
            var mc = GetComponentInParent<MonsterController>();
            if (mc)
                isInverted = mc.InvertedBehaviour;

            Init();

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
            MonsterController.OnForcedBehaviour += HandleOnMonsterForcedBehaviour;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
            MonsterController.OnForcedBehaviour -= HandleOnMonsterForcedBehaviour;
        }

        private void HandleOnMonsterForcedBehaviour(MonsterController monsterController)
        {
            Init();
        }


        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                case PlayerState.Hunter:
                    Init();
                    break;

            }

        }

        void Init()
        {
            Color col;
            if ((PlayerController.Instance.State == PlayerState.Prey && !isInverted) || (PlayerController.Instance.State == PlayerState.Hunter && isInverted))
                col = redLightColor;
            else
                col = blueLightColor;

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