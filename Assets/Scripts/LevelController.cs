using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class LevelController : MonoBehaviour
    {

        float playerChasedTime = 90;

        float playerChasingTime = 10;

        float time;

        float elapsed = 0;

        bool playerChasing = false;



        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                playerChasing = !playerChasing;
                Init();
                
            }
            return;
#endif
            elapsed += Time.deltaTime;
            if (elapsed > time)
            {
                elapsed = 0;
                playerChasing = !playerChasing;

                Init();


            }
        }

        void Init()
        {
            elapsed = 0;
            if (!playerChasing)
                time = playerChasedTime;
            else
                time = playerChasingTime;

            PlayerController.Instance.SetState(!playerChasing ? PlayerState.Prey : PlayerState.Hunter);
        }

        

    }
}