using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;       // plays the animations
    private CharacterMovement movement; // our movement script (for IsGrounded)
    private Rigidbody rb;            // used to read how fast the player moves

    [Header("Double Jump")]
    [SerializeField] private AudioClip doubleJumpSound;
    [SerializeField] private GameObject doubleJumpEffect;

    private AudioSource audioSource;

   private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
 
    private void Update()
    {
        // Send the current speed to the blend tree
        animator.SetFloat("CharacterSpeed", rb.linearVelocity.magnitude);
 
        // On the ground? (controls Falling and ends the flip)
        bool grounded = movement.IsGrounded;
        animator.SetBool("IsGrounded", grounded);
 
        if (grounded)
        {
            // NEW: throw away a double jump request that was never used
            animator.ResetTrigger("doDoubleJump");
        }
        else
        {
            // NEW: Double jump. Space again in mid-air.
            // The flip only plays if PlayerMovement really jumped.
            if (Input.GetButtonDown("Jump") && movement.TryDoubleJump())
            {
                animator.SetTrigger("doDoubleJump");

                // Play the double jump sound
                if (doubleJumpSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(doubleJumpSound);
                }
            }
            // Instantiate the double jump effect
            if (doubleJumpEffect != null)
            {
                Instantiate(doubleJumpEffect, transform.position, Quaternion.identity);
            }
        }
    }
}

