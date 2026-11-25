using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Debug.DrawRay(transform.position, Vector3.down * 1f, Color.red);

    public enum FacingDirection
    {
        left, right
    }

    [Header("References")]
    public Rigidbody2D body2D;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public Vector2 groundCheckOffset = new(0f, -0.65f);
    public Vector2 groundCheckSize = new(0.5f, 0.1f);
    public LayerMask groundCheckMask;

    private float acceleration;
    private float deceleration;

    private float gravity;
    private float jumpVelocity;
    private float terminalVelocity;

    private Vector2 velocity;

    private bool isGrounded = false;
    private bool isJumping = false;

    private FacingDirection facingDirection = FacingDirection.right;

    private Keyboard keyboard;

    void Start()
    {
        body2D.gravityScale = 0;

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVelocity = 2 * apexHeight / apexTime;
        terminalVelocity = gravity;

        keyboard = Keyboard.current;
    }

    void Update()
    {
        CheckForGround();

        Vector2 playerInput = new();

        if (keyboard.rightArrowKey.isPressed)
        {
            playerInput.x += 1;
            facingDirection = FacingDirection.left;
        }

        if (keyboard.leftArrowKey.isPressed)
        {
            playerInput.x += -1;
            facingDirection = FacingDirection.right;
        }

        if (keyboard.upArrowKey.wasPressedThisFrame && isGrounded)
        {
            isJumping = true;
        }

        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x != 0)
        {
            velocity.x += acceleration * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Clamp(velocity.y, terminalVelocity, velocity.y);
        }
        else
        {
            velocity.y = 0;
        }

        if (isJumping)
        {
            velocity.y = jumpVelocity;
            isJumping = false;
        }

        body2D.linearVelocity = velocity;
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(
            transform.position + (Vector3)groundCheckOffset,
            groundCheckSize,
            0 
            );
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public FacingDirection GetFacingDirection()
    {
        return facingDirection;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckOffset, groundCheckSize);
    }
}
