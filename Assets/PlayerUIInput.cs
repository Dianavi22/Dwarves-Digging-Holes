using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerUIInput : MonoBehaviour
{
    private GameManager _gameManager;

    public void OnPause(InputAction.CallbackContext context)
    {
        _gameManager.Pause();
    }

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
