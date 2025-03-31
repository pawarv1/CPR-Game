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
        /*
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        bool isRunning = Input.GetKey(KeyCode.W);

        if (isRunning)
        {
            animator.SetBool("isRunning", true);
            animator.SetFloat("speed", 5);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isWalkingBackward", true);
            animator.SetFloat("speed", 1);
        }
        */

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
