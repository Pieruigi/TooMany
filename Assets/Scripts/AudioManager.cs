using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
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


        // bool playerHasBeenChasing = false;

        // float chasingPitch = 1.25f;

        // float pitchTime = 1f;

        // Start is called before the first frame update
        void Start()
        {
            SwitchToPrey();
        }

        // Update is called once per frame
        void Update()
        {

        }

        // void LateUpdate()
        // {
        //     var playerHasBeenChasingOld = playerHasBeenChasing;
        //     playerHasBeenChasing = PlayerHasBeenChasing();
        //     if (playerHasBeenChasing != playerHasBeenChasingOld)
        //     {
        //         DOTween.KillAll();
        //         if (playerHasBeenChasing)
        //             preySource.DOPitch(chasingPitch, pitchTime);
        //         else
        //             preySource.DOPitch(1, pitchTime);
        //     }

            
        // }

        void OnEnable()
        {
            PlayerController.OnStateChanged += HandleOnPlayerStateChanged;
        }

        void OnDisable()
        {
            PlayerController.OnStateChanged -= HandleOnPlayerStateChanged;
        }

        bool PlayerHasBeenChasing()
        {
            foreach (var monster in MonsterSpawner.Instance.Monsters)
            {
                if (monster.State == MonsterState.Chasing)
                    return true;
            }

            return false;
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