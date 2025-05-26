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
    public static bool SkipWasPressed;

    private static bool canTakeInputs;
    

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _abilityAction;
    private InputAction _switchCamera;
    private InputAction _menuAction; 
    private InputAction _skipAction; 



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
        _skipAction = PlayerInput.actions["Skip"];

        LevelManager.Instance.OnPlayerSpawned.AddListener(() => EnableInputs());
    }

    private void Update()
    {
        if (canTakeInputs)
        {
            if (!(DialogueManager.Instance.dialogueBox.activeSelf))
            {  
                Movement = _moveAction.ReadValue<Vector2>();

                JumpWasPressed = _jumpAction.WasPressedThisFrame();
                JumpIsHeld = _jumpAction.IsPressed();
                JumpWasReleased = _jumpAction.WasReleasedThisFrame();

                RunIsHeld = _runAction.IsPressed();

                AbilityWasPressed = _abilityAction.WasPressedThisFrame();
                AbilityIsHeld = _abilityAction.IsPressed();
                AbilityWasReleased = _abilityAction.WasReleasedThisFrame();

            }
            else
            {
                Movement = Vector2.zero; // Disable movement when in dialogue
                JumpWasPressed = false;
                JumpIsHeld = false;
                JumpWasReleased = false;

                RunIsHeld = false;

                AbilityWasPressed = false;
                AbilityIsHeld = false;
                AbilityWasReleased = false;
            }

            SwitchCameraPressed = _switchCamera.WasPressedThisFrame();
            PauseWasPressed = _menuAction.WasPressedThisFrame();
            SkipWasPressed = _skipAction.WasPressedThisFrame();

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
