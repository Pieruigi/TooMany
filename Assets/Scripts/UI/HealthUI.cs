using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace TMOT
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> hearts;

        RectTransform root;

        float shakeDuration = .5f;
        float shakeStrength = 20;

        void Awake()
        {
            root = transform.GetChild(0) as RectTransform;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

          protected virtual void OnEnable()
        {
            PlayerController.OnPlayerDamaged += HandleOnPlayerDamaged;
            PlayerController.OnPlayerHealed += HandleOnPlayerHealed;
        }

        

        protected virtual void OnDisable()
        {
            PlayerController.OnPlayerDamaged -= HandleOnPlayerDamaged;
            PlayerController.OnPlayerHealed -= HandleOnPlayerHealed;
        }

        private void HandleOnPlayerHealed(float previousHealth, float currentHealth)
        {
            HealPlayer(previousHealth, currentHealth).Forget();
        }

        private void HandleOnPlayerDamaged(float previousHealth, float currentHealth)
        {
            DamagePlayer(previousHealth, currentHealth).Forget();
        }

        private async UniTaskVoid HealPlayer(float previousHealth, float currentHealth)
        {
            // Shake
            root.DOShakePosition(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);

            Debug.Log($"Hearts healing, prev:{previousHealth}, curr:{currentHealth}");
            int count = (int)(currentHealth - previousHealth);
            int startIndex = (int)previousHealth - 1;
            for (int i = 0; i < count; i++)
            {
                hearts[startIndex + i + 1].GetComponent<Animator>().SetTrigger("Heal");
                //yield return new WaitForSeconds(.2f);
                await UniTask.Delay(200);
            }


        }

        private async UniTaskVoid DamagePlayer(float previousHealth, float currentHealth)
        {
            // Shake
            root.DOShakePosition(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);

            Debug.Log($"Hearts damaged, prev:{previousHealth}, curr:{currentHealth}");
            int count = (int)(previousHealth - currentHealth);
            int startIndex = (int)previousHealth - 1;
            for (int i = 0; i < count; i++)
            {
                hearts[startIndex - i].GetComponent<Animator>().SetTrigger("Damage");
                //yield return new WaitForSeconds(.2f);
                await UniTask.Delay(200);
            }
        }

    }
}