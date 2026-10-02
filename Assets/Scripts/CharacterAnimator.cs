using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;       // plays the animations
    private CharacterMovement movement; // our movement script (for IsGrounded)
    private Rigidbody rb;            // used to read how fast the player moves

    private void Start()
    {
        // Find the components on this same GameObject
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Send the current speed to the blend tree (Idle/Walking/Running).
        // Unity 6 calls the Rigidbody's velocity linearVelocity.
        animator.SetFloat("CharacterSpeed", rb.linearVelocity.magnitude);

        // Tell the Animator if we're on the ground (controls Falling)
        animator.SetBool("IsGrounded", movement.IsGrounded);

        // When Fire1 (left mouse button / left Ctrl) is released, roll
        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetTrigger("doFlip");
        }
    }
}
