using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        AudioSource preySource;

        [SerializeField]
        AudioSource hunterSource;

        [SerializeField]
        AudioSource switchSource;




        // Start is called before the first frame update
        void Start()
        {
            SwitchToPrey();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        private void HandleOnPlayerStateChanged(PlayerState oldState, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Prey:
                    SwitchToPrey();
                    break;
                case PlayerState.Hunter:
                    SwitchToHunter();
                    break;
            }
        }

        async void SwitchToPrey()
        {
            if (preySource.isPlaying) return;
            switchSource.Play();
            await Task.Delay(TimeSpan.FromSeconds(.180f));
            hunterSource.Stop();
            preySource.Play();
        }

        async void SwitchToHunter()
        {
            switchSource.Play();
            await Task.Delay(TimeSpan.FromSeconds(.180f));
            hunterSource.Play();
            preySource.Stop();
        }
    }
}