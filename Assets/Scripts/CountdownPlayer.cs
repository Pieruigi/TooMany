using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class CountdownPlayer : MonoBehaviour
    {
        [SerializeField]
        AudioSource source;

        [SerializeField]
        List<AudioClip> clips;

        bool stopping = false;

        UniTask task;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async UniTask Play()
        {
            stopping = false;

            // for (int i = startIndex; i < clips.Count; i++)
            // {
            //     if (stopping)
            //     {
            //         UnityEngine.Debug.Log($"TEST - stopping countdown to {i}");
            //         stopping = false;
            //         source.Stop();
            //         return;
            //     }
            //     UnityEngine.Debug.Log($"TEST - playing countdown {i}");
            //     source.clip = clips[i];
            //     source.Play();
            //     await UniTask.Delay(TimeSpan.FromSeconds(1f));
            // }

            foreach (var clip in clips)
            {
                if (stopping)
                {
                    stopping = false;
                    break;
                }

                source.clip = clip;
                source.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }
        }

        public void Play(int index)
        {
            source.clip = clips[index];
            source.Play();
        }

        public void Stop()
        {
            stopping = true;
            source.Stop();
        }
        

    }
}