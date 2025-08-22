using System;
using System.Collections;
using System.Collections.Generic;
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
            foreach (var clip in clips)
            {
                source.clip = clip;
                source.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }
        }
    }
}