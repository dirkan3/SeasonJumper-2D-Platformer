using UnityEngine;


[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{

    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    [SerializeField] public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    [SerializeField] public float VerticalDeadZoneThreshold = 0.01f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    [SerializeField] public float HorizontalDeadZoneThreshold = 0.01f;


    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14f;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 12f;

    [SerializeField] public float Deceleration = 5;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 0.2f;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30f;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public Vector2 WallJumpForce = new Vector2(20f, 30f);


    [Header("Dash")]
    [Tooltip("The player's dashing power")]
    [SerializeField] public float DashingPower = 39f;

    [Tooltip("the time for the dash")]
    [SerializeField] public float DashingDuration = 0.1f;

    [Tooltip("Cooldown until next dash")]
    [SerializeField] public float DashingCooldown = 3f;


    [Header("gravity/collisin detection")]
    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    [SerializeField] public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    [SerializeField] public float GrounderDistance = 0.05f;



    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    [SerializeField] public float JumpPower = 30f;

    [Tooltip("The maximum vertical movement speed")]
    [SerializeField] public float MaxFallSpeed = 29f;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    [SerializeField] public float FallAcceleration = 88f;

    [Tooltip("The gravity multiplier added when jump is released early")]
    [SerializeField] public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    [SerializeField] public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    [SerializeField] public float JumpBuffer = 0.1f;

    [SerializeField, Range(1, 3)] public int MaxExtraJumps = 1;

    [Tooltip("Tells remaining jumps")]
    public int remainingJumps;
}
