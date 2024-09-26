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
    [SerializeField] private float verticalDeadZone = 0.5f;
    [SerializeField] private float horizontalDeadZone = 0.5f;



    [SerializeField] private Transform _leftRay;

    [SerializeField] private Transform _rightRay;


    private float _horizontal = 0f;
    private float _vertical = 0f;
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool _jumpButtonHeld = false;
    private Vector3 playerVelocity;
    private Rigidbody _rb;

    private PlayerActions _playerActions;

    public bool flip = false;

    public bool flip_vertical = false;
    public bool _isGrounded = false;
    public float gravityScale = 1f;
    public bool carried = false;

    public bool canStopcarried = false;
    public Action forceDetachFunction;

    private readonly float gravityValue = -9.81f;

    public bool JumpJustPressed { get; private set; }
    public bool DashJustPressed { get; private set; }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerActions = GetComponent<PlayerActions>();
    }

    void Update()
    {

        // Move
        if (!_isDashing)
        {
            float xVelocity = _horizontal == 0 && !_isDashingCooldown && carried 
                ? _rb.velocity.x 
                : _horizontal * _speed;
            _rb.velocity = new Vector3(xVelocity, _rb.velocity.y, 0f);            
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
        _isGrounded = Physics.Raycast(_leftRay.position, Vector3.down, 1f) || Physics.Raycast(_rightRay.position, Vector3.down, 1f);
        if (!_isGrounded)
        {
            playerVelocity.y = -2f;
            playerVelocity.y += gravityValue * Time.deltaTime;
            _rb.AddForce(playerVelocity * Time.deltaTime);
        }
        else if(_isGrounded && carried && canStopcarried){
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
        if (!flip)
        {
            // Player faces left
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // Player faces right
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void FlipHoldObject()
    {
        float targetZRotation = 0f;
        float targetYRotation = flip ? 0 : 180;

        if (_vertical > 0)
        {
            targetZRotation = -35f;
        }
        else if (_vertical < 0)
        {
            targetZRotation = 35f;
        }
        _playerActions.StopAnimation();
        _playerActions.CancelInvoke();
        _playerActions.pivot.transform.DORotate(new Vector3(0, targetYRotation, targetZRotation), 0f);
        _playerActions.vertical = _vertical;
        flip_vertical = _vertical != 0;
    }

    #region EVENTS

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpJustPressed = UserInput.instance.JumpJustPressed;
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
        Vector2 vector = context.ReadValue<Vector2>();
        _horizontal = Mathf.Abs(vector.x) > horizontalDeadZone ? vector.x : 0;
        _vertical = Mathf.Abs(vector.y) > verticalDeadZone ? Mathf.RoundToInt(vector.y) : 0;
    }

    public void OnDash()
    {
        DashJustPressed = UserInput.instance.JumpJustPressed;

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
