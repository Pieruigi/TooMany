using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class PlayerAudioManager : MonoBehaviour
    {
        [SerializeField]
        AudioSource hitAudioSource;

        [SerializeField]
        List<AudioClip> hitClips;

        [SerializeField]
        AudioSource deathAudioSource;

        [SerializeField]
        AudioSource deathStaticAudioSource;

        [SerializeField]
        List<AudioClip> deathClips;

        [SerializeField]
        AudioSource moveAudioSource;

        float moveVolume;

        void Awake()
        {
            moveVolume = moveAudioSource.volume;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var player = PlayerController.Instance;

            UpdateMove();
            
            
        }

        void OnEnable()
        {
            PlayerController.OnPlayerDamaged += HandleOnDamaged;
        }

        void OnDisable()
        {
            PlayerController.OnPlayerDamaged -= HandleOnDamaged;
        }

        private void HandleOnDamaged(float previousHealth, float currentHealth)
        {
            PlayHit();


        }

        void PlayHit()
        {
            hitAudioSource.clip = hitClips[UnityEngine.Random.Range(0, hitClips.Count)];
            hitAudioSource.Play();

            if (PlayerController.Instance.State == PlayerState.Dead)
                PlayDeath();
        }

        void PlayDeath()
        {
            deathAudioSource.clip = deathClips[UnityEngine.Random.Range(0, deathClips.Count)];
            deathAudioSource.PlayDelayed(.2f);
            deathStaticAudioSource.PlayDelayed(1f);
        }

        void UpdateMove()
        {
            if (!moveAudioSource.isPlaying) moveAudioSource.Play();

            if (PlayerController.Instance.State == PlayerState.Dead || (PlayerController.Instance.Velocity.magnitude == 0 && !PlayerController.Instance.Rotating))
            {
                if (moveAudioSource.volume > 0)
                {
                    moveAudioSource.volume = 0;
                }
                    
            }
            else
            {
                if (moveAudioSource.volume == 0)
                    moveAudioSource.volume = moveVolume;

                if (PlayerController.Instance.Sprinting)
                    moveAudioSource.pitch = 1.5f;
                else
                    moveAudioSource.pitch = 1f;
            }

            
        }
    }
}