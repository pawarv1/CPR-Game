using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isGivingCPR", true);
        }
        else
        {
            animator.SetBool("isGivingCPR", false);
        }
    }
}
