using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TMOT
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> hearts;

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

        private async void HandleOnPlayerHealed(float previousHealth, float currentHealth)
        {
            Debug.Log($"Hearts healing, prev:{previousHealth}, curr:{currentHealth}");
            int count = (int)(currentHealth - previousHealth);
            int startIndex = (int)previousHealth - 1;
            for (int i = 0; i < count; i++)
            {
                hearts[startIndex + i + 1].GetComponent<Animator>().SetTrigger("Heal");
                //yield return new WaitForSeconds(.2f);
                await Task.Delay(200);
            }
        }

        private async void HandleOnPlayerDamaged(float previousHealth, float currentHealth)
        {
            Debug.Log($"Hearts damaged, prev:{previousHealth}, curr:{currentHealth}");
            int count = (int)(previousHealth - currentHealth);
            int startIndex = (int)previousHealth - 1;
            for (int i = 0; i < count; i++)
            {
                hearts[startIndex - i].GetComponent<Animator>().SetTrigger("Damage");
                //yield return new WaitForSeconds(.2f);
                await Task.Delay(200);
            }
        }

    }
}