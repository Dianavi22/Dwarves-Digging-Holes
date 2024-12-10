using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : EntityMovement
{
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float _dashForce;
    [SerializeField] private Vector2 _deadZoneSpace = new(0.5f, 0.5f);

    [SerializeField] private Transform _leftRay;
    [SerializeField] private Transform _rightRay;

    [SerializeField] ParticleSystem _DashPart;

    private float _vertical = 0f;
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;
    private Animator _animator;
    private Vector3 playerVelocity;
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
    void Start()
    {
        _animator = _p.GetAnimator();
        Stats = GameManager.Instance.Difficulty.PlayerStats;
    }

    override protected void Update()
    {
        base.Update();

        if ((_vertical != 0 && !flip_vertical) || (_vertical == 0 && flip_vertical))
        {
            FlipHoldObject();
        }
    }

    override protected void HandleMovement()
    {
        if (!_isDashing)
        {
            bool isHoldingChariot = _p.HasJoint && Utils.Component.TryGetInParent<GoldChariot>(_p.GetActions().heldObject, out _);
            float xVelocity = _p.IsGrabbed ? _p.GetRigidbody().velocity.x : CalculateXVelocity(horizontalInput, isHoldingChariot);

            _p.GetRigidbody().velocity = new Vector3(xVelocity, _p.GetRigidbody().velocity.y, 0f);

            if (xVelocity != 0)
            {
                if (_movePart.isStopped) _movePart.Play();
            }
            else
            {
                _movePart.Stop();
            }
            _animator?.SetFloat("Run", Mathf.Abs(horizontalInput));
        }
    }

    private float CalculateXVelocity(float horizontalInput, bool isHoldingChariot)
    {
        if (horizontalInput != 0 && !_isDashingCooldown && isHoldingChariot)
        {
            var fatigueReduced = _p.GetFatigue().ReduceCartsFatigue(
                GameManager.Instance.Difficulty.PushCartFatigue.ActionReducer * Time.deltaTime);

            if (!fatigueReduced)
            {
                return _p.GetRigidbody().velocity.x;
            }
        }

        return horizontalInput * Stats.Speed;
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (UIPauseManager.Instance.isPaused) return;

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
                    _p.GetRigidbody().AddForce(Vector3.up * Stats.JumpForce, ForceMode.Impulse);
                    _movePart.Stop();
                }
                break;
            case InputActionPhase.Canceled:
                _jumpButtonHeld = false;
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector = context.ReadValue<Vector2>();
        horizontalInput = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        _vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;
        _animator.SetFloat("Run", Mathf.Abs(horizontalInput));
    }

    public void OnDash(InputAction.CallbackContext _)
    {
        if (_isDashing || _isDashingCooldown || _p.IsGrabbed) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();
        Vector3 dashDirection = flip ? Vector3.right : Vector3.left;
        _p.GetRigidbody().velocity = new Vector3(dashDirection.x * _dashForce, _p.GetRigidbody().velocity.y, 0f);

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

    override protected void HandleJumpPhysics()
    {
        base.HandleJumpPhysics();
        if (_p.GetRigidbody().velocity.y > 0 && !_jumpButtonHeld)
        {
            _p.GetRigidbody().velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
    }
}
