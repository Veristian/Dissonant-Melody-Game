using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static bool PauseWasPressed;
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool AbilityWasPressed;
    public static bool AbilityIsHeld;
    public static bool AbilityWasReleased;
    public static bool SwitchCameraPressed;
    private static bool canTakeInputs;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _abilityAction;
    private InputAction _switchCamera;
    private InputAction _menuAction; 


    private void Awake()
    {


        canTakeInputs = true;
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _abilityAction = PlayerInput.actions["Ability"];
        _switchCamera = PlayerInput.actions["Switch Camera"];
        _menuAction = PlayerInput.actions["Menu"];

        LevelManager.Instance.OnPlayerSpawned.AddListener(() => EnableInputs());
    }

    private void Update()
    {
        if (canTakeInputs)
        {
            Movement = _moveAction.ReadValue<Vector2>();

            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();

            RunIsHeld = _runAction.IsPressed();

            AbilityWasPressed = _abilityAction.WasPressedThisFrame();
            AbilityIsHeld = _abilityAction.IsPressed();
            AbilityWasReleased = _abilityAction.WasReleasedThisFrame();

            SwitchCameraPressed = _switchCamera.WasPressedThisFrame();

            PauseWasPressed = _menuAction.WasPressedThisFrame();
        }
    }

    public static void DisableInputs()
    {
        canTakeInputs = false;
    }
    public static void EnableInputs()
    {
        canTakeInputs = true;
    }
}
