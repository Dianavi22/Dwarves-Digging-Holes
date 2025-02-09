using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using FMODUnity;
using DG.Tweening;

public class PlayerUIInput : MonoBehaviour
{
    private Player _p;

    public EventReference submitEvent;
    public EventReference navigateEvent;
    private UIPauseManager _uiPause;

    private bool condition = true;

    private void Start()
    {
        _p = GetComponent<Player>();
        if (UIPauseManager.Instance == null)
        {
            this.enabled = false;
        } else
        {
            _uiPause = UIPauseManager.Instance;
        }
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _uiPause.Pause();
        _uiPause.scaleButton = true;

    }

    public void OnTest(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _uiPause.isPaused)
            RuntimeManager.PlayOneShot(navigateEvent);
    }

    //! Les actions sont compliquées à gérer, il faudrait revoir ça
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && _uiPause.isPaused && condition)
        {
            condition = false;
            RuntimeManager.PlayOneShot(submitEvent);
            DOVirtual.DelayedCall(0.5f, () => condition = true);
        }
    }
}
