using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;      // units per second
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float speed;     // units per second
    [SerializeField] private Vector3 startPosition; // starting position of the player
    [SerializeField] private float jumpForce = 5f;      // strength of the jump
    [SerializeField] private float doubleJumpForce = 4f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.5f; // ray length
    [SerializeField] private LayerMask groundMask; 
    
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private Rigidbody rb;               // the physics body we move
    private Vector3 moveDirection;      // worked out from input in Update()
    private float turnInput;            // horizontal input for rotation
    private bool isGrounded;            // true when standing on the ground
    private bool jumpRequested = false; // set in Update(), used later
    private bool doubleJumpRequested = false; // set in TryDoubleJump()
    private bool hasDoubleJumped = false;     // one per jump
    private bool isRunning = false;     // true when running, false when walking

    public bool IsGrounded => isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        startPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathPlane"))
        {
            resetLevel();
        }
    }


    private void Update()
    {
        // 1. Ground Check
        isGrounded = Physics.Raycast(transform.position + transform.up*groundDistance/2, -transform.up, groundDistance, groundMask);

        // Back on the ground: the double jump is available again
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // 2. Read Inputs
        turnInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right turn input
        float moveZ = Input.GetAxisRaw("Vertical");  // W/S or Up/Down forward/backward

        // Rotate the player transform in Update for responsive turning visuals
        //transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        //ignore the y axis for movement
        forward.y = 0f;
        right.y = 0f;

        // Calculate forward movement relative to current facing direction
        moveDirection = forward * moveZ + right * turnInput;

        if (moveDirection.magnitude > 0f)
        {
            transform.forward = moveDirection.normalized; // face the direction of movement
        }

        // 3. Handle Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }

        // 4. Handle Run Input
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = true;
            speed = runSpeed;
        }
        else
        {
            isRunning = false;
            speed = moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
 
        if (jumpRequested)
        {
            Jump(jumpForce);
            jumpRequested = false;
        }
 
        // NEW: the second jump, in mid-air
        if (doubleJumpRequested)
        {
            Jump(doubleJumpForce);
            doubleJumpRequested = false;
        }
    }
 
    private void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f,
                                        rb.linearVelocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }

    private void MovePlayer()
    {
        // Calculate velocity based on current forward vector
        Vector3 targetVelocity = moveDirection * speed;

        // Apply movement while preserving vertical velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    // Tries to jump a second time in mid-air.
    // Returns true if the double jump really happened.
    public bool TryDoubleJump()
    {
        // Not allowed on the ground, or if it was already used
        if (isGrounded || hasDoubleJumped) return false;
 
        doubleJumpRequested = true;
        hasDoubleJumped = true;
        return true;
    }

    private void resetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // if a trap is touched the player will lose one health point and when health reaches 0 the level will reset
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            resetLevel();
        }
    }
}
