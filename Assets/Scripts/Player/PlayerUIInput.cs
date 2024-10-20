using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerUIInput : PlayerManager
{
    private UIPauseManager _gameManager;

    void Awake()
    {
        _gameManager = FindObjectOfType<UIPauseManager>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        _gameManager.Pause(this);
    }
}
