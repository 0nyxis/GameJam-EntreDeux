using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Data (left/right)")]
    [SerializeField] bool isInverted = false;
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deceleration = 10f;

    public bool _isInverted => isInverted;

    private float moveInput;

    bool WasLastMoveRight = false;

    [Header("Ground Detection Data")]
    [SerializeField] private Transform groundCheckA;
    [SerializeField] private Transform groundCheckB;

    [SerializeField] private float groundCheckDist = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Jump Data")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpHoldForce = 20f;
    [SerializeField] float maxJumpHoldTime = 0.25f;
    [SerializeField] float gravityScale = 3.0f;
    [SerializeField] float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;
    [SerializeField] float coyoteTime = 0.15f;

    private float coyoteTimeCounter = 0.0f;


    private bool isJumping;
    private bool jumpReleased;
    private float jumpHoldCounter;

    [Header("Animation Data")]
    [SerializeField] SpriteRenderer body;
    [SerializeField] SpriteRenderer mask;
    Animator animator;

    // Inputs Data
    InputAction ia_Move;
    InputAction ia_Jump;
    InputAction ia_Interact;
    public static event Action OnJumpPressed;


    private Rigidbody2D rb;
    private Interact interactScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        interactScript = transform.AddComponent<Interact>();
        ia_Move = InputSystem.actions.FindAction("Move");
        ia_Jump = InputSystem.actions.FindAction("Jump");
        ia_Interact = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = (isInverted) ? ia_Move.ReadValue<Vector2>().x * (-1) : ia_Move.ReadValue<Vector2>().x;

        WasLastMoveRight = moveInput > 0 ? true : (moveInput < 0 ? false : WasLastMoveRight);

        body.flipX = WasLastMoveRight;
        mask.flipX = WasLastMoveRight;

        if (ia_Jump.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
            OnJumpPressed?.Invoke();
        }
        if (ia_Jump.WasReleasedThisFrame())
        {
            jumpReleased = true;
        }
        if (ia_Interact.WasReleasedThisFrame() && interactScript._canInteract)
        {
            StartCoroutine(interactScript.Boom(Camera.main.WorldToViewportPoint(transform.position)));
        }
    }



    void FixedUpdate()
    {
        isGrounded = IsOnGround();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.fixedDeltaTime;

        HandleMovement();
        HandleJump();
        HandleGravity();

        //Updating the animation based on the changed values

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

        //Debug.Log($"Input: {moveInput:F1} | " + $"Grounded {isGrounded} | " + $"Velocity X: {rb.linearVelocity.x:F2} | " + $"Velocity Y: {rb.linearVelocity.y}");
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
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            isJumping = true;
            jumpHoldCounter = maxJumpHoldTime;

            jumpBufferCounter = 0;
        }

        // The player release the jump button -> Stop the "ascension" of the jump
        if (jumpReleased)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

                isJumping = false;
            }
            jumpReleased = false;
            coyoteTimeCounter = 0;
        }

        // The player continue its ascension while he can, by holding the jump button
        if (isJumping && ia_Jump.IsPressed() && jumpHoldCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);

            jumpHoldCounter -= Time.fixedDeltaTime;
        }

        // The player has reached the maximum time for the jump, stopping the jump possibility
        if (jumpHoldCounter <= 0 || rb.linearVelocity.y <= 0)
        {
            isJumping = false;
        }
    }

    private bool IsOnGround()
    {
        RaycastHit2D hitA = Physics2D.Raycast(groundCheckA.position, Vector2.down, groundCheckDist, groundLayer);
        RaycastHit2D hitB = Physics2D.Raycast(groundCheckB.position, Vector2.down, groundCheckDist, groundLayer);

        if (hitA.collider != null || hitB.collider != null)
        {
            if (hitA.collider != null)
            {
                transform.SetParent(hitA.collider.transform, true);
            }
            else
            {
                transform.SetParent(hitB.collider.transform, true);
            }

            return true;
        }

        return false;
    }

    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = gravityScale * 1.5f;
        else
            rb.gravityScale = gravityScale;
    }

    public void InverseControl(bool _a)
    {
        isInverted = _a;
    }
}
