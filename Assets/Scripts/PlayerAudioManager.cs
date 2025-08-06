using System;
using System.Collections;
using System.Collections.Generic;
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
        List<AudioClip> deathClips;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
        }
    }
}