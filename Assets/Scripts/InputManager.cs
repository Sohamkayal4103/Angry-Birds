using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    private InputAction _mousePositionAction;
    private InputAction _mouseAction;

    public static bool WasLeftMouseButtonPressed;
    public static bool WasLeftMouseButtonReleased;
    public static bool IsLeftMousePressed;
    public static Vector2 MousePosition;



    private void Awake()
    {
        _mousePositionAction = PlayerInput.actions["MousePosition"];
        _mouseAction = PlayerInput.actions["Mouse"];
    }


    private void Update()
    {
        MousePosition = _mousePositionAction.ReadValue<Vector2>();
        WasLeftMouseButtonPressed = _mouseAction.WasPressedThisFrame();
        WasLeftMouseButtonReleased = _mouseAction.WasReleasedThisFrame();
        IsLeftMousePressed = _mouseAction.IsPressed();
    }


}
