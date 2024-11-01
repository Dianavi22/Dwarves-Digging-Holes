using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public bool JumpJustPressed { get; private set; }
    public bool DashJustPressed { get; private set; }

    public bool GrabThrowJustPressed { get; private set; }
    public bool BaseActionJustPressed { get; private set; }
    public bool TauntJustPressed { get; private set; }

    public PlayerInput PlayerInput { private set; get; }

    private InputAction _jumpAction;
    private InputAction _grabThrowAction;
    private InputAction _baseAction;
    private InputAction _dashAction;
    private InputAction _tauntAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        SetupInputActions();
    }

    void Update()
    {
        UpdateInput();
    }

    private void SetupInputActions()
    {
        //_jumpAction.ChangeBinding()
        _jumpAction = PlayerInput.actions["Jump"];
        _grabThrowAction = PlayerInput.actions["GrabAndThrow"];
        _baseAction = PlayerInput.actions["BaseAction"];
        _dashAction = PlayerInput.actions["Dash"];
        _tauntAction = PlayerInput.actions["Taunt"];
    }
    private void UpdateInput()
    {
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        GrabThrowJustPressed = _grabThrowAction.WasPressedThisFrame();
        BaseActionJustPressed = _baseAction.WasPressedThisFrame();
        DashJustPressed = _dashAction.WasPressedThisFrame();
        TauntJustPressed = _tauntAction.WasPressedThisFrame();
    }
}
