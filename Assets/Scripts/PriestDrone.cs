using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class PriestDrone : MonoBehaviour
    {
         [Header("Movement Settings")]
    public float topY = 10f;           // Highest vertical position
    public float bottomY = 4f;         // Lowest vertical position
    public float duration = 2f;        // Time to move between top and bottom
    public Ease ease = Ease.InOutSine; // Smooth ease at the ends

    private Tween moveTween;

    void Start()
    {
        // Pick a random starting position between top and bottom
        float randomY = Random.Range(bottomY, topY);
        Vector3 startPos = transform.position;
        startPos.y = randomY;
        transform.position = startPos;

        // Random delay so objects are desynchronized
        float randomDelay = Random.Range(0f, duration);

        // Decide if we start going up or down
        float targetY = (randomY > (topY + bottomY) / 2f) ? bottomY : topY;

        // Create the ping-pong tween
        moveTween = transform.DOMoveY(targetY, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo) // infinite up/down loop
            .SetDelay(randomDelay);      // desynchronization
    }

    void OnDestroy()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }
    }
}