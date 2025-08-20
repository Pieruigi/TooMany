using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class DynamicEmissionRange : MonoBehaviour
    {
        [SerializeField]
        Renderer _renderer;

        [SerializeField]
        int materialIndex;

        public Transform target;

        public float minDistance = 7f;

        public float maxDistance = 32f;

        public float smoothSpeed = 5f;        // Velocità interpolazione range

        Color hdrColor;




        void Awake()
        {
            Init();
      
        }

        void Start()
        {
            target = PlayerController.Instance.transform;
        }

        void Update()
        {
            if (target == null) return;

            float distance = Vector3.Distance(transform.position, target.position);

            // Mappa distanza da 0-maxDistance in range da maxRange a minRange
            Color desiredColor = Color.Lerp(hdrColor, Color.black, 1f - (maxDistance - distance) / (maxDistance - minDistance));


            // Interpola per un movimento fluido
            var color = _renderer.materials[materialIndex].GetColor("_BaseColor");
            color = Color.Lerp(color, desiredColor, Time.deltaTime * smoothSpeed);

            _renderer.materials[materialIndex].SetColor("_BaseColor", color);

        }



        public void Init()
        {
            hdrColor = _renderer.materials[materialIndex].GetColor("_BaseColor");
        }
    }
    
}
