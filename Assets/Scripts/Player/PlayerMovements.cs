using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _dashForce = 10f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 100f;
    [SerializeField] private LayerMask groundLayer;

    private float _horizontal = 0f;
    private bool _isJumping = false;
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;
    private Vector3 playerVelocity;
    private Rigidbody _rb;

    public bool flip = false;
    public bool _isGrounded = false;
    public float gravityScale = 1f;
    public bool carried = false;
    public Action forceDetachFunction;

    private readonly float gravityValue = -9.81f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Move
        if (!_isDashing)
        {
            if( _horizontal == 0 && !_isDashingCooldown)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, 0f);
            }
            else
            {
                _rb.velocity = new Vector3( _horizontal * _speed, _rb.velocity.y, 0f);
            }
        }

        // Flip
        if (_horizontal > 0 && flip || _horizontal < 0 && !flip)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        // Faster falling
        if (_rb.velocity.y < 1 && !_isDashing)
        {
            _rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        // Shorter jump
        else if (_rb.velocity.y > 0 && !_jumpButtonHeld)
        {
            // Apply low jump multiplier to reduce upward velocity when the jump button is released
            _rb.velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        // Grounded
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        if (!_isGrounded)
        {
            playerVelocity.y = -2f;
            playerVelocity.y += gravityValue * Time.deltaTime;
            _rb.AddForce(playerVelocity * Time.deltaTime);
        }
    }

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private void FlipFacingDirection()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    #region EVENTS

    public void OnJump(InputAction.CallbackContext context)
    {
        if(carried)
        {
            forceDetachFunction?.Invoke();
        }
        // When jump is pressed
        if (context.phase == InputActionPhase.Performed && _isGrounded)
        {
            if (_isGrounded && !_isDashing)
            {
                _jumpButtonHeld = true;
                _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
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
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void OnDash()
    {
        if (!_isDashing && !_isDashingCooldown && !carried)
        {
            _isDashing = true;
            _isDashingCooldown = true;

            Vector3 dashDirection = new(flip ? -1 : 1, 0, 0);
            _rb.velocity = new Vector3(dashDirection.x * _dashForce, _rb.velocity.y, 0f);

            DOVirtual.DelayedCall(0.2f, () =>
            {
                _isDashing = false;
                Invoke(nameof(EndDashCoolDown), 0.75f);
            });
        }
    }
    #endregion
}
