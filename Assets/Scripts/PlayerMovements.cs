using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerMovements : MonoBehaviour
{
    private float _horizontal = 0f;
    private Rigidbody _rb;
    private bool _isDashingCooldown = false;

    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private float _dashForce = 10f;

    [SerializeField]
    private float _jumpForce = 10f;

    [SerializeField]
    private LayerMask groundLayer;
    public bool flip = false;
    private bool _isDashing = false;
    private bool _isGrounded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update est appelé à chaque frame
    void Update()
    {
        if (!_isDashing)
        {
            _rb.velocity = new Vector3(_horizontal * _speed, _rb.velocity.y, 0f);
        }

        if (_horizontal > 0 && flip == true || _horizontal < 0 && flip == false)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        // Vérification si le joueur est au sol
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    private void FlipFacingDirection()
    {
        transform.DOFlip();
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;
    }

    // EVENTS //
    
    void OnMove(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();
        _horizontal = inputMovement.x;
    }

    void OnJump()
    {
        if (_isGrounded && !_isDashing)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    void OnDash()
    {
        if (!_isDashing && !_isDashingCooldown)
        {
            _isDashing = true;
            _isDashingCooldown = true;

            float yVelocity = _rb.velocity.y;

            Vector3 dashDirection = new(flip ? -1 : 1, 0, 0);

            _rb.velocity = new Vector3(dashDirection.x * _dashForce, yVelocity, 0f);

            DOVirtual.DelayedCall(0.2f, () =>
            {
                _isDashing = false;
                Invoke(nameof(EndDashCoolDown), 0.75f);
            });
        }
    }

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }
}
