using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField]
        Image staminaFill;

        [SerializeField]
        AudioSource audioSource;

        RectTransform panel;

        float shakeDuration = .5f;
        float shakeStrength = 20f;

        float lastStamina = 0;

        void Awake()
        {
            panel = transform.GetChild(0) as RectTransform;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            var pos = (staminaFill.transform as RectTransform).anchoredPosition;
            pos.x = Mathf.Lerp(-232f, 0f, PlayerController.Instance.Stamina / 1f);
            (staminaFill.transform as RectTransform).anchoredPosition = pos;

            // Shake if the player is holding the sprint key and the stamina depletes
            if (lastStamina > 0 && PlayerController.Instance.Stamina == 0)
                panel.DOShakePosition(shakeDuration, shakeStrength);

            // Shake if the player hit the sprint key with no stamina
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (PlayerController.Instance.Stamina == 0)
                {
                    // Shake        
                    panel.DOShakePosition(shakeDuration, shakeStrength);
                    audioSource.Play();
                }
            }


            lastStamina = PlayerController.Instance.Stamina;
        }
    }
}