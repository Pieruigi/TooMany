using UnityEngine;

public class LightSwing : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Oscillation Settings")]
    public Axis swingAxis = Axis.X;
    public float swingAmplitude = 10f;     // Gradi di rotazione
    public float swingSpeed = 1f;          // Frequenza del movimento

    [Header("Random Offset (optional)")]
    public bool addRandomPhase = true;

    private float baseAngle;
    private float phaseOffset;

    void Start()
    {
        baseAngle = GetAxisAngle();
        phaseOffset = addRandomPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed + phaseOffset) * swingAmplitude;
        SetAxisAngle(baseAngle + angle);
    }

    private float GetAxisAngle()
    {
        switch (swingAxis)
        {
            case Axis.X: return transform.localEulerAngles.x;
            case Axis.Y: return transform.localEulerAngles.y;
            case Axis.Z: return transform.localEulerAngles.z;
            default: return 0f;
        }
    }

    private void SetAxisAngle(float angle)
    {
        Vector3 rot = transform.localEulerAngles;
        switch (swingAxis)
        {
            case Axis.X: rot.x = angle; break;
            case Axis.Y: rot.y = angle; break;
            case Axis.Z: rot.z = angle; break;
        }
        transform.localEulerAngles = rot;
    }
}
