using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : Player
{
    [Header("Values")]
    [SerializeField] private float _speed;
    [SerializeField] private float _dashForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private Vector2 _deadZoneSpace = new (0.5f, 0.5f);

    [SerializeField] private Transform _leftRay;
    [SerializeField] private Transform _rightRay;

    [SerializeField] ParticleSystem _DashPart;

    private float _horizontal = 0f;
    private float _vertical = 0f;
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;
    private Vector3 playerVelocity;

    public bool flip = false;

    public bool flip_vertical = false;
    public bool _isGrounded = false;
    public float gravityScale = 1f;

    //TODO remove this and use PlayerAction.carried instead
    public bool carried = false;
    public bool canStopcarried = false;

    public Action forceDetachFunction;

    private readonly float gravityValue = -9.81f;

    void Update()
    {
        // Move
        if (!_isDashing)
        {
            float xVelocity = _horizontal == 0 && !_isDashingCooldown && carried
                ? rb.velocity.x
                : _horizontal * _speed;
            rb.velocity = new Vector3(xVelocity, rb.velocity.y, 0f);
        }

        // Flip
        if (_horizontal > 0 && flip || _horizontal < 0 && !flip)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        if ((_vertical != 0 && !flip_vertical) || (_vertical == 0 && flip_vertical))
        {
            FlipHoldObject();
        }

        // Faster falling
        if (rb.velocity.y < 1 && !_isDashing)
        {
            rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        // Shorter jump
        else if (rb.velocity.y > 0 && !_jumpButtonHeld)
        {
            // Apply low jump multiplier to reduce upward velocity when the jump button is released
            rb.velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        // Grounded
        _isGrounded = Physics.Raycast(_leftRay.position, Vector3.down, 1f) || Physics.Raycast(_rightRay.position, Vector3.down, 1f);
        if (!_isGrounded)
        {
            playerVelocity.y = -2f;
            playerVelocity.y += gravityValue * Time.deltaTime;
            rb.AddForce(playerVelocity * Time.deltaTime);
        }
        else if (carried && canStopcarried)
        {
            carried = false;
            canStopcarried = false;
        }
    }

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;

        transform.rotation = Quaternion.Euler(0, flip ? 0 : 180, 0);
    }

    private void FlipHoldObject()
    {
        float targetYRotation = flip ? 0 : 180;
        float targetZRotation = -Math.Sign(_vertical) * 35f;

        actions.StopAnimation();
        actions.CancelInvoke();
        actions.pivot.transform.DORotate(new Vector3(0, targetYRotation, targetZRotation), 0f);
        actions.vertical = _vertical;
        flip_vertical = _vertical != 0;
    }

    #region EVENTS

    public void OnJump(InputAction.CallbackContext context)
    {
        if (carried)
        {
            forceDetachFunction?.Invoke();
        }
        // When jump is pressed
        if (context.phase == InputActionPhase.Performed && _isGrounded)
        {

            if (_isGrounded && !_isDashing)
            {
                _jumpButtonHeld = true;
                rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        // When jump button is released
        if (context.phase == InputActionPhase.Canceled)
        {
            _jumpButtonHeld = false;
        }
        else if (context.phase == InputActionPhase.Started)
        {
            _jumpButtonHeld = true;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector = context.ReadValue<Vector2>();
        _horizontal = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        _vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;
    }

    public void OnDash(InputAction.CallbackContext _)
    {
        if (_isDashing || _isDashingCooldown || carried) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();
        Vector3 dashDirection = flip ? Vector3.left : Vector3.right;
        rb.velocity = new Vector3(dashDirection.x * _dashForce, rb.velocity.y, 0f);

        DOVirtual.DelayedCall(0.2f, () =>
        {
            _isDashing = false;
            Invoke(nameof(EndDashCoolDown), 0.75f);
            _DashPart.Stop();
        });
    }
    #endregion
}
