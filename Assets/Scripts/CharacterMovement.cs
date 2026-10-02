using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
   [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;      // units per second
    [SerializeField] private float turnSpeed = 150f;    // degrees per second
    [SerializeField] private float jumpForce = 5f;      // strength of the jump

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.5f; // ray length
    [SerializeField] private LayerMask groundMask;        // ground layers

    private Rigidbody rb;               // the physics body we move
    private Vector3 moveDirection;      // worked out from input in Update()
    private float turnInput;            // horizontal input for rotation
    private bool isGrounded;            // true when standing on the ground
    private bool jumpRequested = false; // set in Update(), used later

    public bool IsGrounded => isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        // 1. Ground Check
        isGrounded = Physics.Raycast(transform.position + transform.up*groundDistance/2, -transform.up, groundDistance, groundMask);

        // 2. Read Inputs
        turnInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right turn input
        float moveZ = Input.GetAxisRaw("Vertical");  // W/S or Up/Down forward/backward

        // Rotate the player transform in Update for responsive turning visuals
        transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);

        // Calculate forward movement relative to current facing direction
        moveDirection = transform.forward * moveZ;

        // 3. Handle Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    private void MovePlayer()
    {
        // Calculate velocity based on current forward vector
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Apply movement while preserving vertical velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
}
