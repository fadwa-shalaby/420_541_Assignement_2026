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
    [SerializeField] private float maxChargeTime = 3f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.5f; // ray length
    [SerializeField] private LayerMask groundMask; 
    
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    private Rigidbody rb;               // the physics body we move
    public Vector3 moveDirection;      // worked out from input in Update()
    private float turnInput;            // horizontal input for rotation
    private bool isGrounded;            // true when standing on the ground
    private bool jumpRequested = false; // set in Update(), used later
    private bool doubleJumpRequested = false; // set in TryDoubleJump()
    private bool hasDoubleJumped = false;     // one per jump
    private bool isRunning = false;     // true when running, false when walking
    private float chargeTime = 0.0f;
    private bool isCharging = false;
    private float jumpForceToApply;

    private PlayerPowerUps powerUps; // reference to the PlayerPowerUps script

    public bool IsGrounded => isGrounded;
    public int CurrentHealth => currentHealth;
    public float SpeedMultiplier { get; set; } = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerUps = GetComponent<PlayerPowerUps>();
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
        moveDirection = moveDirection.normalized ;

        // 3. Handle Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded && !isCharging)
        {
            chargeTime = 0.0f;
            isCharging = true;
        }

        if (Input.GetButton("Jump") && isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0.0f, maxChargeTime);

            // Automatically jump after 3 seconds
            if (chargeTime >= maxChargeTime)
            {
                ExecuteChargedJump();
            }
        }

        if (Input.GetButtonUp("Jump") && isCharging)
        {
            ExecuteChargedJump();
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
            Jump(jumpForceToApply);
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

    private void ExecuteChargedJump()
    {
       if (!isCharging || !isGrounded)
        return;

        float chargePercentage = chargeTime / maxChargeTime;

        float finalJumpForce = Mathf.Lerp(jumpForce, jumpForce * 2.5f, chargePercentage);

        jumpForceToApply = finalJumpForce;
        jumpRequested = true;

        isCharging = false;
        chargeTime = 0.0f;
    }

    private void MovePlayer()
    {
        // Calculate velocity based on current forward vector
        Vector3 targetVelocity = moveDirection * speed * SpeedMultiplier;

        // Apply movement while preserving vertical velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    // Tries to jump a second time in mid-air.
    // Returns true if the double jump really happened.
    public bool TryDoubleJump()
    {
        // Not allowed on the ground, or if it was already used
        if (isGrounded || hasDoubleJumped || powerUps == null || !powerUps.HasJumpBoost) return false;
 
        doubleJumpRequested = true;
        hasDoubleJumped = true;
        return true;
    }

    public void resetLevel()
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
