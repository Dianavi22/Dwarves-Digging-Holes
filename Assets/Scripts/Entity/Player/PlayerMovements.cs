using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : EntityMovement
{
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private Vector2 _deadZoneSpace = new(0.5f, 0.5f);

    [SerializeField] private Transform _leftRay;
    [SerializeField] private Transform _rightRay;

    [SerializeField] ParticleSystem _DashPart;

    private float _vertical = 0f;
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;
    public bool flip_vertical = false;

    public Action forceDetachFunction;

    Player _p => (Player)GetBase;

    private bool _playGroundedPart = true;
    [SerializeField] ParticleSystem _groundedPart;
    [SerializeField] ParticleSystem _movePart;

    void Awake()
    {
        GetBase = GetComponent<Player>();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        // If low jump, fall faster 
        // Note: Dunno why but velocity.y on the chariot is > to 0
        if (!isGrounded && _p.GetRigidbody().velocity.y > 0 && !_jumpButtonHeld)
            FasterFalling(lowJumpMultiplier);
    }

    protected new void Update()
    {
        base.Update();

        if ((_vertical != 0 && !flip_vertical) || (_vertical == 0 && flip_vertical))
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

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;

        transform.rotation = Quaternion.Euler(0, flip ? 0 : 180, 0);
    }

    private void FlipHoldObject()
    {
        float targetZRotation = -Math.Sign(_vertical) * 35f;

        if (_p.GetActions().pivot.transform.localEulerAngles.z == targetZRotation) return;

        //_p.GetActions().StopAnimation();
        //_p.GetActions().CancelInvoke();
        _p.GetActions().pivot.transform.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0f);
        _p.GetActions().vertical = _vertical;
        flip_vertical = _vertical != 0;
    }

    #region EVENTS
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector = context.ReadValue<Vector2>();
        float _horizontal = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        _vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;

        CanMove = PlayerCanMove(Mathf.Abs(_horizontal) > 0);
        //Debug.Log(CanMove);
        Move(_horizontal);

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
        if (!GameManager.Instance.isInMainMenu && UIPauseManager.Instance.isPaused) return;

        if (_p.IsGrabbed)
        {
            forceDetachFunction?.Invoke();
        }
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (isGrounded && !_isDashing)
                {
                    _jumpButtonHeld = true;
                    Jump();
                    _movePart.Stop();
                }
                break;
            case InputActionPhase.Canceled:
                _jumpButtonHeld = false;
                break;
        }
    }
    public void OnDash(InputAction.CallbackContext _)
    {
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
}
