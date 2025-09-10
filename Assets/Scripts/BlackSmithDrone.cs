using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlackSmithDrone : MonoBehaviour
{
 [Header("Rotation Settings")]
    public Vector3 targetRotation = new Vector3(0, 45, 0); // Target rotation in degrees
    public float duration = 1.5f;                          // Time to reach the target rotation
    public float delayBetweenLoops = 0.5f;                 // Delay between ping-pong loops

    private Vector3 initialRotation;                       // To store the starting rotation
    private Tween rotationTween;                           // Reference to the tween for control

    [SerializeField]
    AudioSource clinckSource;

    [SerializeField]
    ParticleSystem fx;

    void Start()
    {
        // Save the initial rotation of the GameObject
        initialRotation = transform.eulerAngles;

        Rotate();
    }

    void Rotate()
    {
        transform
            .DOLocalRotate(targetRotation, duration, RotateMode.Fast)
            .SetEase(Ease.InOutBounce)     // Apply bounce effect for robotic feel
            .SetLoops(2, LoopType.Yoyo)   // Infinite ping-pong loop
            .SetDelay(delayBetweenLoops)  // Delay before repeating the loop
            .OnComplete(() => { Rotate(); });

        clinckSource.PlayDelayed(delayBetweenLoops + 0.6f);
        fx.Play();
    }

    
}
