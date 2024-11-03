using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : EntityMovement
{
    [SerializeField] private float _dashForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private Vector2 _deadZoneSpace = new (0.5f, 0.5f);

    [SerializeField] private Transform _leftRay;
    [SerializeField] private Transform _rightRay;

    [SerializeField] ParticleSystem _DashPart;

    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;

    public float gravityScale = 1f;

    public Action forceDetachFunction;

    private Player _p;

    private void Awake()
    {
        _p = GetComponent<Player>();
        SetRigidbody(_p.GetRigidbody());
    }

    void FixedUpdate()
    {
        if (!_isDashing && !_p.IsCarried)
            Move();

        // Faster falling
        if (_p.GetRigidbody().velocity.y < 1 && !_isDashing)
        {
            _p.GetRigidbody().velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        // Shorter jump
        else if (_p.GetRigidbody().velocity.y > 0 && !_jumpButtonHeld)
        {
            // Apply low jump multiplier to reduce upward velocity when the jump button is released
            _p.GetRigidbody().velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
    }

    private new void Update()
    {
        base.Update();
        FlipHoldObject();
    }

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private void FlipHoldObject()
    {
        //float targetYRotation = HDirection < 0 ? 0 : 180;
        float targetZRotation = -VDirection * 35f;

        _p.GetActions().StopAnimation();
        _p.GetActions().CancelInvoke();
        _p.GetActions().pivot.transform.localEulerAngles = Vector3.forward * targetZRotation;
        _p.GetActions().vertical = VDirection;
    }

    #region EVENTS

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_p.IsCarried)
        {
            forceDetachFunction?.Invoke();
        }
        // When jump is pressed
        if (context.phase == InputActionPhase.Performed && !_isDashing)
        {
            _jumpButtonHeld = true;
            Jump();
        }

        // When jump button is released
        if (context.phase == InputActionPhase.Canceled)
        {
            _jumpButtonHeld = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector = context.ReadValue<Vector2>();
        float horizontal = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        float vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;
        SetGrid(horizontal, vertical);
    }

    public void OnDash(InputAction.CallbackContext _)
    {
        if (_isDashing || _isDashingCooldown || _p.IsCarried) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();
        _p.GetRigidbody().velocity = new Vector3(HDirection * _dashForce, _p.GetRigidbody().velocity.y, 0f);

        DOVirtual.DelayedCall(0.2f, () =>
        {
            _isDashing = false;
            Invoke(nameof(EndDashCoolDown), 0.75f);
            _DashPart.Stop();
        });
    }
    #endregion
}
