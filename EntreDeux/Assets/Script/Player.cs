using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Movement Data (left/right)
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deceleration = 10f;

    private float moveInput;

    // Ground Detection Data
    [SerializeField] private Transform groundCheckA;
    [SerializeField] private Transform groundCheckB;

    [SerializeField] private float groundCheckDist = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;


    // Jump Data
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpHoldForce = 20f;
    [SerializeField] float maxJumpHoldTime = 0.25f;
    [SerializeField] float gravityScale = 3.0f;

    private bool isJumping;
    private bool jumpQueued;
    private bool jumpReleased;

    private float jumpHoldCounter;

    // Inputs Data
    InputAction ia_Move;
    InputAction ia_Jump;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ia_Move = InputSystem.actions.FindAction("Move");
        ia_Jump = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = ia_Move.ReadValue<Vector2>().x;

        if (ia_Jump.WasPressedThisFrame())
            jumpQueued = true;
        if (ia_Jump.WasReleasedThisFrame())
            jumpReleased = true;
    }

    void FixedUpdate()
    {
        isGrounded = IsOnGround();
        HandleMovement();
        HandleJump();
        HandleGravity();
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput * maxSpeed;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }
    private void HandleJump()
    {
        //Can jump
        if(jumpQueued && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            isJumping = true;
            jumpHoldCounter = maxJumpHoldTime;
        }

        jumpQueued = false;

        // The player release the jump button -> Stop the "ascension" of the jump
        if(jumpReleased)
        {
            if(rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

                isJumping = false;
            }
            jumpReleased = false;
        }

        // The player continue its ascension while he can, by holding the jump button
        if(isJumping && ia_Jump.IsPressed() && jumpHoldCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);

            jumpHoldCounter -= Time.fixedDeltaTime;
        }

        // The player has reached the maximum time for the jump, stopping the jump possibility
        if(jumpHoldCounter <= 0 || rb.linearVelocity.y <= 0)
        {
            isJumping = false;
        }
    }

    private bool IsOnGround()
    {
        RaycastHit2D hitA = Physics2D.Raycast(groundCheckA.position, Vector2.down, groundCheckDist, groundLayer);
        RaycastHit2D hitB = Physics2D.Raycast(groundCheckB.position, Vector2.down, groundCheckDist, groundLayer);

        return (hitA.collider != null || hitB.collider != null);
    }


    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = gravityScale * 1.5f;
        else
            rb.gravityScale = gravityScale;
    }
}
