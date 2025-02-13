using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance;

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

    public static PlayerInput PlayerInput;

    // Static input states
    public static Vector2 movement;
    public static bool jumpWasPressed;
    public static bool jumpIsHeld;
    public static bool jumpWasReleased;
    public static bool runIsHeld;
    public static bool dashWasPressed;
    public static bool downWasPressed;
    public static bool LadderIsHeld;
    public static bool InteractionWasPressed;
    public static bool HoldWasPressed;
    public static bool FlashLightWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _downAction;
    private InputAction _LadderAction;
    private InputAction _interactionAction;
    private InputAction _holdAction;
    private InputAction _FlashLightAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _dashAction = PlayerInput.actions["Dash"];
        _downAction = PlayerInput.actions["DownPlatform"];
        _LadderAction = PlayerInput.actions["Ladder"];
        _interactionAction = PlayerInput.actions["Interaction"];
        _holdAction = PlayerInput.actions["Hold"];
        _FlashLightAction = PlayerInput.actions["FlashLight"];
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _jumpAction.Enable();
        _runAction.Enable();
        _dashAction.Enable();
        _downAction.Enable();
        _LadderAction.Enable();
        _interactionAction.Enable();
        _holdAction.Enable();
        _FlashLightAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _runAction.Disable();
        _dashAction.Disable();
        _downAction.Disable();
        _LadderAction.Disable();
        _interactionAction.Disable();
        _holdAction.Disable();
        _FlashLightAction.Disable();
    }

    private void Update()
    {
        jumpWasPressed = false;
        jumpWasReleased = false;

        movement = _moveAction.ReadValue<Vector2>();

        LadderIsHeld = _LadderAction.IsPressed();

        jumpWasReleased = _jumpAction.WasReleasedThisFrame();

        jumpWasPressed = _jumpAction.WasPressedThisFrame();
        dashWasPressed = _dashAction.WasPressedThisFrame();
        downWasPressed = _downAction.WasPressedThisFrame();
        InteractionWasPressed = _interactionAction.WasPressedThisFrame();
        HoldWasPressed = _holdAction.WasPressedThisFrame();
        FlashLightWasPressed = _FlashLightAction.WasPressedThisFrame();

        jumpIsHeld = _jumpAction.IsPressed();
        runIsHeld = _runAction.IsPressed();
    }
}
