using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class DynamicParticleRange : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        ParticleSystem _particleSystem;

        
        float maxScale = 1;
        float minScale = 0;           // Range minimo (luce spenta o quasi)

        [SerializeField]
        float minDistance = 7f;
        
        //[SerializeField]
        float maxDistance = 20f;       // Distanza oltre cui la luce si spegne

        [SerializeField]
        float smoothSpeed = 5f;        // Velocità interpolazione range

        float maxIntensity = 0;

        

        void Awake()
        {
            maxScale = transform.localScale.x;
        }

        // Start is called before the first frame update
        void Start()
        {
            target = PlayerController.Instance.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            float distance = Vector3.Distance(transform.position, target.position);

            // Mappa distanza da 0-maxDistance in range da maxRange a minRange
            float desiredScale = Mathf.Lerp(maxScale, minScale, 1f - (maxDistance - distance) / (maxDistance - minDistance));
            desiredScale = Mathf.Clamp(desiredScale, minScale, maxScale);

            float scale = transform.localScale.x;
            scale = Mathf.Lerp(scale, desiredScale, Time.deltaTime * smoothSpeed);
            transform.localScale = Vector3.one * scale;

            if (scale > 0.001f)
            {
                if (!_particleSystem.isPlaying && _particleSystem.main.loop)
                    _particleSystem.Play();
            }
            else
            {
                if (_particleSystem.isPlaying && _particleSystem.main.loop)
                    _particleSystem.Stop();
            }

            
        }
    }
}