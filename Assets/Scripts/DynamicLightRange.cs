using TMOT;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DynamicLightRange : MonoBehaviour
{
    public Transform target;              // Il player o oggetto da seguire
    public float maxRange = 10f;          // Range massimo luce quando vicino
    public float minRange = 0f;           // Range minimo (luce spenta o quasi)
    public float maxDistance = 15f;       // Distanza oltre cui la luce si spegne
    public float smoothSpeed = 5f;        // Velocità interpolazione range

    public Renderer volumetric;
    private Light lightSource;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        maxRange = lightSource.range;
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
        float desiredRange = Mathf.Lerp(maxRange, minRange, distance / maxDistance);
        desiredRange = Mathf.Clamp(desiredRange, minRange, maxRange);

        // Interpola per un movimento fluido
        lightSource.range = Mathf.Lerp(lightSource.range, desiredRange, Time.deltaTime * smoothSpeed);
        if (volumetric)
            volumetric.material.SetFloat("_Intensity", Mathf.Lerp(0.34f, 0, distance / maxDistance));
    }
}
