using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using FMODUnity;
using DG.Tweening;

public class PlayerUIInput : MonoBehaviour
{
    private Player _p;

    private UIPauseManager _uiPause;

    private void Start()
    {
        _p = GetComponent<Player>();
        if (UIPauseManager.Instance == null && !GameManager.Instance.isInMainMenu)
        {
            this.enabled = false;
        } else
        {
            _uiPause = UIPauseManager.Instance;
        }
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.isInMainMenu) return;

        if (context.phase == InputActionPhase.Started)
            _uiPause.Pause();
        _uiPause.scaleButton = true;

    }

    public void OnTest(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }

    //! Les actions sont compliquées à gérer, il faudrait revoir ça
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            if(GameManager.Instance.isInMainMenu || _uiPause.isPaused) RuntimeManager.PlayOneShot(GameManager.Instance.GetSubmitUISound());
    }
}
