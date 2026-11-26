
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float groundCheckTimer;
    public float coyoteTimeCounter;
    public float moveSpeed = 10f;
    Collider2D player;
    ContactFilter2D filter = new ContactFilter2D();

    private float gravity;
    private float jumpVelocity;
    private float terminalVelocity = 0;
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    private float acceleration;
    private float deceleration;

    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;
    bool isJumping = false;
    private Vector2 velocity;

    public float coyoteTime = 5f;
    public bool isGrounded = false;

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {

        player = GetComponent<Collider2D>();

        rb = GetComponent<Rigidbody2D>();

        filter.SetLayerMask(LayerMask.GetMask("Ground"));

        filter.useNormalAngle = true;
        filter.minNormalAngle = 45;
        filter.maxNormalAngle = 135;

        rb.gravityScale = 0;

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVelocity = 2 * apexHeight / apexTime;
        terminalVelocity = gravity;
    }

    void Update()
    {

        if (IsGrounded())
        {
            if (groundCheckTimer > 0.1f)
            {
                coyoteTimeCounter = coyoteTime;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        Vector2 playerInput = new Vector2();

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            playerInput.x = -1;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            playerInput.x = 1;
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame && coyoteTimeCounter > 0)
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
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Clamp(velocity.y, terminalVelocity, velocity.y);
        }
        else
        {
            if (groundCheckTimer > 0.1f)
            {
                velocity.y = 0;
            }
        }

        if (isJumping)
        {
            velocity.y = jumpVelocity;
            coyoteTimeCounter = 0f;
            isJumping = false;
        }
        print(velocity);

        rb.linearVelocity = velocity;

        if (IsGrounded())
        {
            groundCheckTimer = 0;
        }
        else
        {
            groundCheckTimer += Time.fixedDeltaTime;
        }
    }



    public bool IsWalking()
    {
        return rb.linearVelocity.x > 0.1f || rb.linearVelocity.x < -0.1f;
    }
    public bool IsGrounded()
    {
        return player.IsTouching(filter);
    }

    public FacingDirection GetFacingDirection()
    {
        return (rb.linearVelocity.x > 0.1f) ? FacingDirection.right : (rb.linearVelocity.x < -0.1f) ? FacingDirection.left : FacingDirection.left;
    }
}
