using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Stop()
        {
            stopping = true;
            source.Stop();
        }
    }
}