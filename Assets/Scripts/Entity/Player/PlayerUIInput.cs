using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using FMODUnity;

public class PlayerUIInput : MonoBehaviour
{
    private Player _p;

    public EventReference submitEvent;
    public EventReference navigateEvent;

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

    public void OnNavigate() {
        RuntimeManager.PlayOneShot(navigateEvent);
    }
    public void OnSubmit() {
        RuntimeManager.PlayOneShot(submitEvent);
    }
}
