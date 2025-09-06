using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class TestFreeze : MonoBehaviour
{

    Animator animator;

    bool walking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (walking)
                animator.SetTrigger("idle");
            else
                animator.SetTrigger("walk");
            walking = !walking;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (animator.speed == 0)
                animator.speed = 1;
            else
                animator.speed = 0;
        }
        
    }
}
