using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public sealed class PonyoMovement2D : MonoBehaviour
{
    [Header("Auto-run")]
    [SerializeField, Min(0f)] private float runSpeed = 3.2f;

    [Header("Jump")]
    [SerializeField, Min(0f)] private float jumpSpeed = 7.8f;
    [SerializeField, Min(0f)] private float jumpHoldAcceleration = 16f;
    [SerializeField, Min(0f)] private float maximumJumpHoldTime = 0.18f;
    [SerializeField, Min(0f)] private float baseGravityScale = 3f;
    [SerializeField, Min(1f)] private float releasedJumpGravityMultiplier = 1.7f;
    [SerializeField, Min(1f)] private float fallingGravityMultiplier = 1.35f;
    [SerializeField, Min(0f)] private float maximumFallSpeed = 18f;
    [SerializeField, Min(0f)] private float coyoteTime = 0.12f;
    [SerializeField, Min(0f)] private float jumpBufferTime = 0.15f;
    [SerializeField] private Transform groundProbe;
    [SerializeField, Min(0.01f)] private float groundProbeRadius = 0.16f;
    [SerializeField] private LayerMask groundMask = ~0;

    private Rigidbody2D body;
    private float speedMultiplier = 1f;
    private float lastGroundedTime = float.NegativeInfinity;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private float jumpStartedTime = float.NegativeInfinity;
    private bool jumpHeld;
    private bool movementEnabled = true;

    public float RunSpeed => runSpeed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool pressed = Keyboard.current?.spaceKey.wasPressedThisFrame == true
            || Mouse.current?.leftButton.wasPressedThisFrame == true
            || Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true;
        bool held = Keyboard.current?.spaceKey.isPressed == true
            || Mouse.current?.leftButton.isPressed == true
            || Touchscreen.current?.primaryTouch.press.isPressed == true;

        if (pressed)
        {
            lastJumpPressedTime = Time.time;
        }

        jumpHeld = held;
    }

    private void FixedUpdate()
    {
        if (!movementEnabled)
        {
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
            return;
        }

        if (groundProbe != null && Physics2D.OverlapCircle(groundProbe.position, groundProbeRadius, groundMask) != null)
        {
            lastGroundedTime = Time.time;
        }

        Vector2 velocity = body.linearVelocity;
        velocity.x = runSpeed * speedMultiplier;

        float gravityMultiplier = 1f;
        if (velocity.y < -0.01f)
        {
            gravityMultiplier = fallingGravityMultiplier;
        }
        else if (velocity.y > 0.01f && !jumpHeld)
        {
            gravityMultiplier = releasedJumpGravityMultiplier;
        }

        body.gravityScale = baseGravityScale * gravityMultiplier;

        bool bufferedJump = Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool canUseCoyoteTime = Time.time - lastGroundedTime <= coyoteTime;
        if (bufferedJump && canUseCoyoteTime)
        {
            velocity.y = jumpSpeed;
            lastJumpPressedTime = float.NegativeInfinity;
            lastGroundedTime = float.NegativeInfinity;
            jumpStartedTime = Time.time;
        }
        else if (jumpHeld && velocity.y > 0f && Time.time - jumpStartedTime <= maximumJumpHoldTime)
        {
            velocity.y += jumpHoldAcceleration * Time.fixedDeltaTime;
        }

        velocity.y = Mathf.Max(velocity.y, -maximumFallSpeed);

        body.linearVelocity = velocity;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    public void Bounce(float verticalSpeed)
    {
        Vector2 velocity = body.linearVelocity;
        velocity.y = Mathf.Max(velocity.y, verticalSpeed);
        body.linearVelocity = velocity;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundProbe == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundProbe.position, groundProbeRadius);
    }
}
