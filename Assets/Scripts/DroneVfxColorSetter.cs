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

        bool isBlue = false;


        // Start is called before the first frame update
        void Start()
        {
            var mc = GetComponentInParent<MonsterController>();
            if (mc)
                isBlue = mc.InvertedBehaviour;

            Init();

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Init()
        {
            Color col;
            if (!isBlue)
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