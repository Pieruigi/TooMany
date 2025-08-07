using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class DroneAudioManager : MonoBehaviour
    {
        [SerializeField]
        AudioSource explosionAudioSource;

        [SerializeField]
        List<AudioClip> explosionClips;

        [SerializeField]
        AudioSource talkAudioSource;

        [SerializeField]
        List<AudioClip> talkClips;

        
        bool isChasing = false;

        float talkTime = 0;
        bool talk = false;


        void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (talkTime > 0)
                talkTime -= Time.deltaTime;

            if (talk && talkTime <= 0 && !talkAudioSource.isPlaying)
            {
                talkAudioSource.clip = talkClips[UnityEngine.Random.Range(0, talkClips.Count)];
                talkAudioSource.Play();
                talkTime = talkAudioSource.clip.length + UnityEngine.Random.Range(2f, 3f);
            }
        }

      

        void OnEnable()
        {
            MonsterController.OnExploded += HandleOnMonsterDestroyed;
            CustomDroneController.OnExploded += HandleOnCustomDroneDestroyed;
            MonsterController.OnStateChanged += HandleOnMonsterStateChanged;
        }

        void OnDisable()
        {
            MonsterController.OnExploded -= HandleOnMonsterDestroyed;
            CustomDroneController.OnExploded -= HandleOnCustomDroneDestroyed;
            MonsterController.OnStateChanged -= HandleOnMonsterStateChanged;
        }

        private void HandleOnMonsterStateChanged(MonsterController monsterController, MonsterState oldState, MonsterState newState)
        {
            if (!CompareTag("Monster")) return;
            var dc = GetComponent<MonsterController>();
            if (dc == null || dc != monsterController) return;
            talk = newState == MonsterState.Chasing;

           
        }

        private void HandleOnCustomDroneDestroyed(CustomDroneController drone)
        {
            Debug.Log($"TEST - Handling on exploded {drone.gameObject.name}");
            if (!CompareTag("Mule")) return;
            var dc = GetComponent<CustomDroneController>();
            if (dc == null || dc != drone) return;

            PlayExplosion();
        }

        private void HandleOnMonsterDestroyed(MonsterController monsterController)
        {
            if (!CompareTag("Monster")) return;

            var mc = GetComponent<MonsterController>();
            if (mc == null || mc != monsterController) return;

            PlayExplosion();
        }

        void PlayExplosion()
        {
            Debug.Log($"TEST - Play explosion fx");
            explosionAudioSource.clip = explosionClips[UnityEngine.Random.Range(0, explosionClips.Count)];
            explosionAudioSource.Play();
        }
    }
}