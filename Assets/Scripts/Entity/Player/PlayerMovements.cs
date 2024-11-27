using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerMovements : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _speed;
    [SerializeField] private float _dashForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private Vector2 _deadZoneSpace = new(0.5f, 0.5f);

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

    public Action forceDetachFunction;

    private readonly float gravityValue = -9.81f;

    private Player _p;

    private bool _playGroundedPart = true;
    [SerializeField] ParticleSystem _groundedPart;
    [SerializeField] ParticleSystem _movePart;


    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    void Update()
    {
        // Move
        if (!_isDashing)
        {
            //bool canMoveChariot = _p.HasJoint && Utils.TryGetParentComponent<GoldChariot>(_p.GetActions().heldObject, out _);

            // || (canMoveChariot && _p.GetFatigue().ReduceCartsFatigue(GameManager.Instance.Difficulty.PlayerPushFatigue * Time.deltaTime))
            float xVelocity = _horizontal == 0 && !_isDashingCooldown && !_p.IsCarried
                ? _p.GetRigidbody().velocity.x
                : _horizontal * _speed;
            _p.GetRigidbody().velocity = new Vector3(xVelocity, _p.GetRigidbody().velocity.y, 0f);
            _movePart.Play();


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

        // Grounded
        _isGrounded = Physics.Raycast(_leftRay.position, Vector3.down, 1f) || Physics.Raycast(_rightRay.position, Vector3.down, 1f);
        if (!_isGrounded)
        {
            _movePart.Stop();
            _playGroundedPart = false;
            playerVelocity.y = -2f;
            playerVelocity.y += gravityValue * Time.deltaTime;
            _p.GetRigidbody().AddForce(playerVelocity * Time.deltaTime);
        }
        if (_isGrounded && !_playGroundedPart)
        {
            _playGroundedPart = true;
            _groundedPart.Play();
        }
    }

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;

        transform.rotation = Quaternion.Euler(0, flip ? 0 : 180, 0);
    }

    private void FlipHoldObject()
    {
        float targetZRotation = -Math.Sign(_vertical) * 35f;

        _p.GetActions().StopAnimation();
        _p.GetActions().CancelInvoke();
        _p.GetActions().pivot.transform.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0f);
        _p.GetActions().vertical = _vertical;
        flip_vertical = _vertical != 0;
    }

    #region EVENTS

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_p.IsCarried)
        {
            forceDetachFunction?.Invoke();
        }
        // When jump is pressed
        if (context.phase == InputActionPhase.Performed && _isGrounded)
        {

            if (_isGrounded && !_isDashing)
            {
                _jumpButtonHeld = true;
                _p.GetRigidbody().AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
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
        if (_isDashing || _isDashingCooldown || _p.IsCarried) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();
        Vector3 dashDirection = flip ? Vector3.left : Vector3.right;
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
}
