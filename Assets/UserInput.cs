using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{

    public static UserInput instance;

    public bool JumpJustPressed { get; private set; }
    private PlayerInput _playerInput;
    private InputAction _jumpAction;

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
    }
    private void UpdateInput()
    {
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
    }
}
