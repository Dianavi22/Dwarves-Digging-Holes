using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _dashForce = 10f;
    [SerializeField] private float _jumpForce = 10f; 

    private float _horizontal = 0f;
    private bool _isJumping = false;
    private bool _isDashingCooldown = false;

    [Header("Controls")]
    private Vector2 movementInput = Vector2.zero;
    private CharacterController controller;

    [SerializeField]
    private LayerMask groundLayer;
    public bool flip = false;
    public bool _isGrounded = false;

    private Vector3 playerVelocity;
    public float gravityScale = 1f; 
    private float gravityValue = -9.81f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(move * Time.deltaTime * _speed);

        //if (!_isDashing)
        //{
        //    // _rb.velocity = new Vector3(_horizontal * _speed, _rb.velocity.y, 0f);
        //}

        if (_horizontal > 0 && flip || _horizontal < 0 && !flip)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        if (_isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }
        playerVelocity.y += gravityValue * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.triggered && _isGrounded)
        {
            Jump();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(_jumpForce * -2f * gravityValue);
        _isGrounded = false;
        _isJumping = true;
    }

    //private void OnDash()
    //{
    //    if (!_isDashing && !_isDashingCooldown)
    //    {
    //        _isDashing = true;
    //        _isDashingCooldown = true;

    //        Vector3 dashDirection = new Vector3(flip ? -1 : 1, 0, 0);
    //        controller.Move(dashDirection * _dashForce * Time.deltaTime);

    //        DOVirtual.DelayedCall(0.2f, () =>
    //        {
    //            _isDashing = false;
    //            Invoke(nameof(EndDashCoolDown), 0.75f);
    //        });
    //    }
    //}

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private void FlipFacingDirection()
    {
        transform.DOFlip();
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;
    }
}
