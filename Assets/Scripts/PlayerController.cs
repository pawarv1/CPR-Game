using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Blend", 0);
        animator.SetFloat("BlendY", 0);
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        animator.SetFloat("Blend",horizontalInput);
        animator.SetFloat("BlendY", verticalInput);
        
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        
        if (movement.magnitude > 0)
        {
            movement.Normalize();
        }
        
        transform.Translate(movement * moveSpeed * Time.deltaTime);
        
        float movementMagnitude = movement.magnitude;
        animator.SetFloat("Speed", movementMagnitude);
        
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isGivingCPR");
        }
        else
        {
    
        }
    }
}
