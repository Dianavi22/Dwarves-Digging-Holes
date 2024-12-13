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

    private bool condition = true;

    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.isInMainMenu) return;
        if (context.phase == InputActionPhase.Started)
            UIPauseManager.Instance.Pause(_p);
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.isInMainMenu) return;
        if (context.phase == InputActionPhase.Started && UIPauseManager.Instance.isPaused)
            RuntimeManager.PlayOneShot(navigateEvent);
    }

    //! Les actions sont compliquées à gérer, il faudrait revoir ça
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.isInMainMenu) return;
        if (context.phase == InputActionPhase.Performed && UIPauseManager.Instance.isPaused && condition)
        {
            condition = false;
            RuntimeManager.PlayOneShot(submitEvent);
            DOVirtual.DelayedCall(0.5f, () => condition = true);

        }
    }
}
