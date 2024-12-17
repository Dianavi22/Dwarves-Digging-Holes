using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : EntityMovement
{
    [SerializeField] private Vector2 _deadZoneSpace = new(0.5f, 0.5f);

    [Header("Particle effect")]
    [SerializeField] ParticleSystem _DashPart;
    [SerializeField] ParticleSystem _groundedPart;
    [SerializeField] ParticleSystem _movePart;

    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool flip_vertical = false;

    public Action forceDetachFunction;

    Player _p => (Player)GetBase;

    private bool _playGroundedPart = true;

    void Awake()
    {
        GetBase = GetComponent<Player>();
    }

    protected new void Update()
    {
        base.Update();

        if ((_moveInput.y != 0 && !flip_vertical) || (_moveInput.y == 0 && flip_vertical))
        {
            FlipHoldObject();
        }
    }

    private bool PlayerCanMove(bool isInputActivated)
    {
        if (!GameManager.Instance.isInMainMenu)
        {
            bool isHoldingChariot = _p.HasJoint && Utils.Component.TryGetInParent<GoldChariot>(_p.GetActions().heldObject, out _);
            if (isInputActivated && isHoldingChariot)
                return !_p.IsGrabbed && _p.GetFatigue().ReduceCartsFatigue(
                    GameManager.Instance.Difficulty.PushCartFatigue.ActionReducer * Time.deltaTime);
        }

        return !_p.IsGrabbed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Enemy>(collision.collider, out var enemy) && _isDashing)
        {
            enemy.HandleDestroy();
        }
    }

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;

        transform.rotation = Quaternion.Euler(0, flip ? 0 : 180, 0);
    }

    private void FlipHoldObject()
    {
        float targetZRotation = -Math.Sign(_moveInput.y) * 35f;

        if (_p.GetActions().pivot.transform.localEulerAngles.z == targetZRotation) return;

        //_p.GetActions().StopAnimation();
        //_p.GetActions().CancelInvoke();
        _p.GetActions().pivot.transform.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0f);
        _p.GetActions().vertical = _moveInput.y;
        flip_vertical = _moveInput.y != 0;
    }

    #region EVENTS
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanDoAnything()) return;

        Vector2 vector = context.ReadValue<Vector2>();
        float _horizontal = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        float _vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;

        CanMove = PlayerCanMove(Mathf.Abs(_horizontal) > 0);
        //Debug.Log(CanMove);
        Move(new Vector2(_horizontal, _vertical));

        if (CanMove && Mathf.Abs(_horizontal) > 0)
        {
            if (_movePart.isStopped) _movePart.Play();
        }
        else
        {
            _movePart.Stop();
        }

        _p.GetAnimator().SetFloat("Run", _horizontal);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!CanDoAnything()) return;
        
        if (_p.IsGrabbed)
        {
            forceDetachFunction?.Invoke();
        }
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (isGrounded && !_isDashing)
                {
                    IsPerformingJump = true;
                    Jump();
                    _movePart.Stop();
                }
                break;
            case InputActionPhase.Canceled:
                IsPerformingJump = false;
                break;
        }
    }
    public void OnDash(InputAction.CallbackContext _)
    {
        if (!CanDoAnything()) return;

        if (_isDashing || _isDashingCooldown || _p.IsGrabbed) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();

        Dash();

        DOVirtual.DelayedCall(0.2f, () =>
        {
            _isDashing = false;
            Invoke(nameof(EndDashCoolDown), 0.75f);
            _DashPart.Stop();
        });
    }
    #endregion

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private bool CanDoAnything()
    {
        return !GameManager.Instance.isGameOver && !GameManager.Instance.isInMainMenu && !UIPauseManager.Instance.isPaused;
    }
}
