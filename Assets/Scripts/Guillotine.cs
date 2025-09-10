using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Guillotine : MonoBehaviour
{
    
 [Header("Local Z positions")]
    float topZ = 0f;      // Local Z at the top (starting point)
    public float bottomZ = -5f;  // Local Z at the bottom (impact point)

    [Header("Timings")]
    public float dropDuration = 0.3f;  // Fast fall
    public float impactPause = 0.5f;   // Wait at bottom
    public float riseDuration = 1.0f;  // Slow rise
    public float topPause = 1.5f;      // Wait at top

    private Sequence seq;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    ParticleSystem particle;


    void Start()
    {
        // Force starting at the top
        Vector3 startPos = transform.localPosition;
        //startPos.z = topZ;
        topZ = startPos.z;
        //transform.localPosition = startPos;

        // Build sequence
        seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveZ(bottomZ, dropDuration).SetEase(Ease.OutBounce)) // fall with bounce
           .AppendInterval(impactPause)                                                  // pause at bottom
           .Append(transform.DOLocalMoveZ(topZ, riseDuration).SetEase(Ease.InOutSine))   // rise up smoothly
           .AppendInterval(topPause)                                                     // pause at top
           .SetLoops(-1);

        StartCoroutine(PlayVFX());                                                             // loop forever
    }

    IEnumerator PlayVFX()
    {
        while (true)
        {
            yield return new WaitForSeconds(dropDuration-.4f);
            audioSource.Play(); particle.Play();
            yield return new WaitForSeconds(impactPause + riseDuration + topPause + .4f);

        }
    }

    void OnDestroy()
    {
        if (seq != null) seq.Kill();
    }
}
