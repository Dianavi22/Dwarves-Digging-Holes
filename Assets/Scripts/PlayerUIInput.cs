using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerUIInput : MonoBehaviour
{
    private UIPauseManager _gameManager;

    public void OnPause(InputAction.CallbackContext context)
    {
        _gameManager.Pause(this.gameObject);
    }

    void Awake()
    {
        _gameManager = FindObjectOfType<UIPauseManager>();
    }
}
