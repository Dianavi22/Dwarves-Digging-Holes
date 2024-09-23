using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{

    public static UserInput instance;

    public bool JumpJustPressed { get; private set; }
    public bool DashJustPressed { get; private set; }

    public bool GrabThrowJustPressed { get; private set; }
    public bool BaseActionJustPressed { get; private set; }
    public bool TauntJustPressed { get; private set; }


    private PlayerInput _playerInput;

    private InputAction _jumpAction;
    private InputAction _grabThrowAction;
    private InputAction _baseAction;
    private InputAction _dashAction;
    private InputAction _tauntAction;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        _playerInput = GetComponent<PlayerInput>();
        SetupInputActions();
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdateInput();
    }

    private void SetupInputActions()
    {
        _jumpAction = _playerInput.actions["Jump"];
        _grabThrowAction = _playerInput.actions["GrabAndThrow"];
        _baseAction = _playerInput.actions["BaseAction"];
        _dashAction = _playerInput.actions["Dash"];
        _tauntAction = _playerInput.actions["Taunt"];
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
