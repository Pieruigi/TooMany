using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class DestroyedBot : MonoBehaviour
    {
        [SerializeField]
        DynamicEmissionRange dynamicEmissionRange;
        
        Light targetLight;

        [SerializeField]
        float minIntensity = 0f;
        float maxIntensity = 2f;
        float minDelay = 0.05f;
        float maxDelay = 0.3f;

        [SerializeField]
        Color minHdrColor = Color.black;

        DynamicLightRange dynamicLightRange;

        Color maxHdrColor;

        void Awake()
        {
            dynamicLightRange = GetComponent<DynamicLightRange>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (targetLight == null)
                targetLight = GetComponent<Light>();

            maxIntensity = dynamicLightRange.MaxIntensity;
            maxHdrColor = dynamicEmissionRange.MaxHdrColor;

            StartFlicker();
        }

        // Update is called once per frame
        void Update()
        {

        }


        void StartFlicker()
        {
            Sequence seq = DOTween.Sequence();

            // Create a random number to randomize the blink pattern
            int flickerCount = Random.Range(2, 6);

            for (int i = 0; i < flickerCount; i++)
            {
                float wait = Random.Range(minDelay, maxDelay);

                // On/off or intensity
                seq.AppendCallback(() =>
                {
                    if (Random.value > 0.5f)
                    {
                        dynamicLightRange.MaxIntensity = minIntensity;
                        dynamicEmissionRange.MaxHdrColor = minHdrColor;
                    }
                    else
                    {
                        var r = Random.Range(0f, 1f);

                        dynamicLightRange.MaxIntensity = Mathf.Lerp(minIntensity, maxIntensity, r); //Random.Range(minIntensity, maxIntensity);
                        dynamicEmissionRange.MaxHdrColor = Color.Lerp(minHdrColor, maxHdrColor, r);
                    }
                        
                });

                seq.AppendInterval(wait);
            }

            // Restart
            seq.OnComplete(StartFlicker);
        }
    }
}