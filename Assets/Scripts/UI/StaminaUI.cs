using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField]
        Image staminaFill;

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
        }
    }
}