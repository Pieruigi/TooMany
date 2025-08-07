using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT.UI
{
    public class UIAudioManager : SingletonPersistent<UIAudioManager>
    {
        [SerializeField]
        AudioSource enterAudioSource;

        [SerializeField]
        AudioSource clickAudioSource;

        [SerializeField]
        AudioSource exitAudioSource;

        [SerializeField]
        AudioSource failedAudioSource;

        public void PlayEnter()
        {
            enterAudioSource.Play();
        }

        public void PlayExit()
        {
            exitAudioSource.Play();
        }

        public void PlayClick()
        {
            clickAudioSource.Play();
        }

        public void PlayFailed()
        {
            failedAudioSource.Play();
        }
    }
}