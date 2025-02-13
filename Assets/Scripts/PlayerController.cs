using System;
using System.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private SpriteRenderer _sprite;
    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    public TrailRenderer tr;
    public TrailRenderer Ntr;

    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private float _time;
    public int facingDirection = 1;

    public event Action<int> DirectionChanged;

    private static PlayerController _instance;
    AudioManager audioManager;
    public float FrameVelocityX => _frameVelocity.x;



    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        _time += Time.deltaTime;
        GatherInput();
        HandleSpriteDirection();




        if (_grounded)
        {
            hasLeftWall = false;
            OnLanding();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        CheckCollisions();
        HandleJump();

        WallSlide();
        StopWallJumping();

        HandleDirection();
        HandleGravity();
        ApplyMovement();
        WallJump();

    }

    private void GatherInput()
    {
        Vector2 movement = InputManager.movement;

        if (_stats.SnapInput)
        {
            // Apply dead zone threshold for movement (horizontal and vertical)
            movement.x = Mathf.Abs(movement.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(movement.x);
            movement.y = Mathf.Abs(movement.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(movement.y);
        }

        _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, movement.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);

        animator.SetFloat("speed", MathF.Abs(_frameVelocity.x));


        if (InputManager.jumpWasPressed && !_jumpToConsume)
        {
            animator.SetBool("isJumping", true);
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;

        }

        if (InputManager.dashWasPressed && canDash)
        {
            StartCoroutine(Dash());
        }
    }




    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.BoxCast(_col.bounds.center, _col.bounds.size, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.BoxCast(_col.bounds.center, _col.bounds.size, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            _stats.remainingJumps = _stats.MaxExtraJumps;
            OnLanding();
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            animator.SetBool("isJumping", true);
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !InputManager.jumpIsHeld && _rb.velocity.y > 0)
            _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        // Handle wall jump if sliding
        if (isWallSliding)
        {
            WallJump();
            _jumpToConsume = false;  // Consume input after jump
            return;
        }

        // Normal jump logic
        if (_grounded || CanUseCoyote || _stats.remainingJumps > 0)
        {
            animator.SetBool("isJumping", true);
            ExecuteJump();
        }

        _jumpToConsume = false;  // Consume input after normal jump
    }

    public void ExecuteJump(float extraForce = 0f)
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;

        if (!_grounded)
        {
            _stats.remainingJumps--;
        }
        _frameVelocity.y = _stats.JumpPower + extraForce;
        Jumped?.Invoke();
        audioManager.PlaySFX(audioManager.jump);
    }





    #endregion

    #region WallJumping/sliding

    private float WallSlidingSpeed = 2f;
    private bool isWallSliding;
    private bool isWallJumping;
    private float lastWallJumpTime;
    private const float WallJumpCooldown = 0.2f;
    public float wallJumpingDuration = 0.4f;
    private bool hasLeftWall;
    private bool noStickToWall;
    private const float NoStickDuration = 0.2f;
    private float movementLockTime = 0.1f;
    private float lastWallJumpVelocityTime;
    private bool inputLockedAfterWallJump;
    private float wallJumpLockEndTime;
    private bool isFacingRight = true;

    private bool IsWalled()
    {
        bool walled = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

        Debug.DrawRay(wallCheck.position, isFacingRight ? Vector2.right * 0.5f : Vector2.left * 0.5f, walled ? Color.green : Color.red);

        return walled;
    }

    private void WallSlide()
    {
        if (noStickToWall) return;

        bool walled = IsWalled();

        if (!_grounded && walled)
        {
            isWallSliding = true;
            _frameVelocity.y = Mathf.Clamp(_frameVelocity.y, -WallSlidingSpeed, float.MaxValue);
            Debug.Log("Wall sliding active");
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void WallJump()
    {
        if (!isWallSliding || !_jumpToConsume) return;

        Ntr.emitting = false;

        Debug.Log("Wall jump executed");

        float wallJumpDirection = -facingDirection;


        Vector2 wallJumpForce = new(wallJumpDirection * _stats.WallJumpForce.x * 0.8f, _stats.WallJumpForce.y * 0.8f);

        _rb.velocity = Vector2.zero;  // Reset velocity
        _rb.AddForce(wallJumpForce, ForceMode2D.Impulse);

        // Flip sprite if needed
        if ((wallJumpDirection > 0 && !isFacingRight) || (wallJumpDirection < 0 && isFacingRight))
        {
            FlipSprite();
        }
        audioManager.PlaySFX(audioManager.jump);

        // Update states
        isWallSliding = false;
        isWallJumping = true;
        inputLockedAfterWallJump = true;
        wallJumpLockEndTime = Time.time + 0.3f;

        Invoke(nameof(ResetNoStick), NoStickDuration);
        _jumpToConsume = false;
        lastWallJumpTime = Time.time;
    }

    private void ResetNoStick()
    {
        noStickToWall = false;
    }

    private void StopWallJumping()
    {
        if (isWallJumping && Time.time - lastWallJumpTime > wallJumpingDuration)
        {
            isWallJumping = false;
        }

    }

    #endregion


    #region Helpers

    public void UpdateWallCheckPosition()
    {
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, wallCheck.localPosition.z);
    }

    private void HandleSpriteDirection()
    {
        if (isWallJumping) return;

        float moveX = InputManager.movement.x;

        // Flip when changing directions
        if ((moveX > 0 && !isFacingRight) || (moveX < 0 && isFacingRight))
        {
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        facingDirection = isFacingRight ? 1 : -1;

        // Flip the sprite
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        // Update the wall check position
        UpdateWallCheckPosition();
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }

    #endregion


    #region Dash

    private bool canDash = true;
    private bool isDashing;

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;

        Vector2 dashDirection = InputManager.movement.normalized;
        _frameVelocity = new Vector2(dashDirection.x * _stats.DashingPower, 0f);

        audioManager.PlaySFX(audioManager.dash);

        tr.emitting = true;

        float dashTimer = 0f;
        while (dashTimer < _stats.DashingDuration)
        {
            dashTimer += Time.deltaTime;
            _rb.velocity = _frameVelocity;

            yield return null;
        }

        tr.emitting = false;
        _rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(_stats.DashingCooldown);
        canDash = true;
    }

    #endregion

    #region Horizontal


    private float _displacementXSmoothing;
    private void HandleDirection()
    {
        if (isDashing) return;

        float inputX = InputManager.movement.x;
        float targetVelocityX = inputX * _stats.MaxSpeed;

        if (inputX != 0 && Mathf.Sign(inputX) != Mathf.Sign(_frameVelocity.x))
        {
            _frameVelocity.x = 0;
            _displacementXSmoothing = 0;
        }

        if (Mathf.Abs(inputX) < _stats.HorizontalDeadZoneThreshold)
        {
            float deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.SmoothDamp(_frameVelocity.x, 0, ref _displacementXSmoothing, deceleration);
        }
        else
        {
            float timeSinceWallJump = Time.time - lastWallJumpTime;
            float accelerationFactor = Mathf.Lerp(0.4f, 1f, timeSinceWallJump / 0.5f);
            float acceleration = (_grounded ? _stats.Acceleration : _stats.FallAcceleration) * accelerationFactor;
            _frameVelocity.x = Mathf.SmoothDamp(_frameVelocity.x, targetVelocityX, ref _displacementXSmoothing, acceleration);
        }

        if (Mathf.Abs(_frameVelocity.x) < 0.05f)
        {
            _frameVelocity.x = 0;
        }
    }


    #endregion

    #region Gravity
    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            bool isAscending = _rb.velocity.y > 0;

            float gravityScale = isWallJumping
                ? (isAscending ? _stats.FallAcceleration * 0.4f : _stats.FallAcceleration * 1.3f)
                : _stats.FallAcceleration;

            if (_endedJumpEarly && _frameVelocity.y > 0) gravityScale *= _stats.JumpEndEarlyGravityModifier;

            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, gravityScale * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement()
    {
        if (Time.time < wallJumpLockEndTime)
        {
            _rb.velocity = new Vector2(_rb.velocity.x * 0.8f, _rb.velocity.y);
        }
        else
        {
            inputLockedAfterWallJump = false;
            _rb.velocity = _frameVelocity;
        }
    }
}
