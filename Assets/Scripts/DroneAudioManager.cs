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
            MonsterController.OnExploded += HandleOnMonsterDestroyed;
            CustomDroneController.OnExploded += HandleOnCustomDroneDestroyed;
        }

        void OnDisable()
        {
            MonsterController.OnExploded -= HandleOnMonsterDestroyed;
            CustomDroneController.OnExploded -= HandleOnCustomDroneDestroyed;
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