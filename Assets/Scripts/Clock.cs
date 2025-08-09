using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace TMOT
{
    public class Clock : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> secondHandles;
        [SerializeField]
        List<GameObject> minuteHandles;

        

        const float secondTime = 2f;

        float minuteTime = 60f / secondTime;

        float elapsed = 0;

        void Awake()
        {
            int angle = 40;
            int steps = 360 / angle;
            float time = 0.1f;
            foreach (var h in secondHandles)
            {
                h.transform.localEulerAngles = Vector3.forward * Random.Range(0, steps) * angle;
                DG.Tweening.Sequence seq = DOTween.Sequence();

                for (int i = 0; i < steps; i++)
                {
                    float a = angle * (i + 1);
                    a *= h.name.EndsWith("B") ? -1 : 1;
                    seq.Append(h.transform.DOLocalRotate(Vector3.forward * a, time, RotateMode.Fast).SetEase(Ease.OutBack));
                }

                seq.SetLoops(-1);
            }

            angle /= 10;
            steps = 360 / angle;
            foreach (var h in minuteHandles)
            {
                h.transform.localEulerAngles = Vector3.forward * Random.Range(0, steps) * angle;
                DG.Tweening.Sequence seq = DOTween.Sequence();

                for (int i = 0; i < steps; i++)
                {
                    float a = angle * (i + 1);
                    a *= h.name.EndsWith("B") ? -1 : 1;
                    seq.Append(h.transform.DOLocalRotate(Vector3.forward * a, time, RotateMode.Fast).SetEase(Ease.OutBack));
                }

                seq.SetDelay(time).SetLoops(-1);
            }
                
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}