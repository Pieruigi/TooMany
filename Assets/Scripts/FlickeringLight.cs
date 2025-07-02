using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0.4f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.05f;

    private Light lightSource;
    private float timer;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
            timer = flickerSpeed;
        }
    }
}
