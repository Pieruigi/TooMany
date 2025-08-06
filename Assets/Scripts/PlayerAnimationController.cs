using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]
        Animator animator;

        bool useRightPunch = false;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.O))
                animator.SetTrigger("PunchLeft");
#endif

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

        void OnEnable()
        {
            PlayerController.OnPunch += HandleOnPlayerPunch;
            PlayerController.OnPlayerDamaged += HandleOnPlayerDamaged;
        }

        void OnDisable()
        {
            PlayerController.OnPunch -= HandleOnPlayerPunch;
            PlayerController.OnPlayerDamaged -= HandleOnPlayerDamaged;
        }

        private void HandleOnPlayerDamaged(float previousHealth, float currentHealth)
        {
            if (PlayerController.Instance.State == PlayerState.Dead)
                animator.SetBool("Dead", true);
            animator.SetTrigger("Hit");
            
        }

        private void HandleOnPlayerPunch()
        {
            string triggerName = useRightPunch ? "PunchRight" : "PunchLeft";
            useRightPunch = !useRightPunch;
            animator.SetTrigger(triggerName);
        }
    }
}