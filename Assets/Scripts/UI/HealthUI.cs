using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace TMOT
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> hearts;

        [SerializeField]
        Sprite fullHeartSprite, emptyHeartSprite;


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
            float duration = .2f;
            float strength = 5f;
            for (int i = 0; i < count; i++)
            {
                //hearts[startIndex + i + 1].GetComponent<Animator>().SetTrigger("Heal");

                hearts[startIndex + i + 1].transform.DOShakeScale(duration, strength);
                SetHearthSprite(hearts[startIndex + i + 1], fullHeartSprite, duration * 2f / 3f).Forget();

                //yield return new WaitForSeconds(.2f);
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }


        }

        async UniTaskVoid SetHearthSprite(GameObject heart, Sprite sprite, float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            heart.GetComponent<Image>().sprite = sprite;
        }

        private async UniTaskVoid DamagePlayer(float previousHealth, float currentHealth)
        {
            // Shake
            root.DOShakePosition(shakeDuration, shakeStrength).SetEase(Ease.InOutElastic);

            Debug.Log($"Hearts damaged, prev:{previousHealth}, curr:{currentHealth}");
            int count = (int)(previousHealth - currentHealth);
            int startIndex = (int)previousHealth - 1;
            float duration = .2f;
            float strength = 5f;
            for (int i = 0; i < count; i++)
            {
                //hearts[startIndex - i].GetComponent<Animator>().SetTrigger("Damage");
                hearts[startIndex - i].transform.DOShakeScale(duration, strength);
                SetHearthSprite(hearts[startIndex - i], emptyHeartSprite, duration * 2f / 3f).Forget();
                //yield return new WaitForSeconds(.2f);
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }
        }

    }
}