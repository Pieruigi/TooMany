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

        bool playing = false;
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
            if (playing) return;

            playing = true;

            foreach (var clip in clips)
            {
                if (!playing) return;

                source.clip = clip;
                source.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }
        }

        public void Stop()
        {
            if (!playing) return;
            playing = false;
            source.Stop();
        }
    }
}