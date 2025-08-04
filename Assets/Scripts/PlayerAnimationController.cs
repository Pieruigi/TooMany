using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            PlayerController player = PlayerController.Instance;

            if (player.State == PlayerState.Dead) return;

            // Check is moving
            bool isMoving = false;
            if (player.Velocity.magnitude > 0)
                isMoving = true;

            if (isMoving)
            {
                animator.SetFloat("SprintMul", player.Sprinting ? player.SprintMultiplier : 1f);
                if (!animator.GetBool("Move"))
                    animator.SetBool("Move", true);
            }
            else
            {
                if (animator.GetBool("Move"))
                    animator.SetBool("Move", false);
            }
                 
        }

       
    }
}