using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerUIInput : MonoBehaviour
{
    private Player _p;

    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        UIPauseManager.Instance.Pause(_p);
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }
}
