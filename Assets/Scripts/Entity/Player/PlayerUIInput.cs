using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerUIInput : Player
{
    public void OnPause(InputAction.CallbackContext context)
    {
        UIPauseManager.Instance.Pause(this);
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }
}
