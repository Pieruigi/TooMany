using System;
using TMOT;
using UnityEngine;

public class DynamicLightRange : MonoBehaviour
{
    public Transform target;
    float maxRange = 10f;  
    float minRange = 0f;

    public float minDistance = 7f;

    public float maxDistance = 20f;      

    public float smoothSpeed = 5f;        // Velocità interpolazione range

    float maxIntensity = 0;

    public GameObject volumetric;

    Vector3 volumetricScaleDefault;
    float volumetricFactor = 1;
    


    private Light lightSource;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        maxRange = lightSource.range;
        maxIntensity = lightSource.intensity;


        if (volumetric)
            volumetricScaleDefault = volumetric.transform.localScale;
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
        float desiredRange = Mathf.Lerp(maxRange, minRange, 1f - (maxDistance - distance) / (maxDistance - minDistance));
        desiredRange = Mathf.Clamp(desiredRange, minRange, maxRange);

        float desiredIntensity = Mathf.Lerp(maxIntensity, 0, 1f - (maxDistance - distance ) / (maxDistance - minDistance));
        desiredIntensity = Mathf.Clamp(desiredIntensity, 0, maxIntensity);

        // Interpola per un movimento fluido
        lightSource.range = Mathf.Lerp(lightSource.range, desiredRange, Time.deltaTime * smoothSpeed);
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, desiredIntensity, Time.deltaTime * smoothSpeed);
       
        if (volumetric)
        {
            var volumetricDistance = maxDistance * .55f;
            var desiredFactor = Mathf.Lerp(1, 0, distance / volumetricDistance);
            desiredFactor = Mathf.Clamp(desiredFactor, 0, 1);

            volumetricFactor = Mathf.Lerp(volumetricFactor, desiredFactor, Time.deltaTime * smoothSpeed);
            volumetric.transform.localScale = volumetricScaleDefault * volumetricFactor;
        }
    }
}
