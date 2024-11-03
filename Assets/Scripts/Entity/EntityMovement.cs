using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("EntityMovement")]
    [SerializeField] private float _speed;
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_Decceleration;
    [SerializeField] private float m_VelocityPower;

    [Header("EntityJump")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float m_GroundedOffset = 0.1f;

    private bool _isGrounded = true;

    private Rigidbody _rb;

    protected float HDirection => Math.Sign(_horizontal);
    private float _horizontal = 0f;
    protected float VDirection => Math.Sign(_vertical);
    private float _vertical = 0f;

    private 

    protected void SetRigidbody(Rigidbody rb)
    {
        _rb = rb;
    }

    protected void Update()
    {
        #region Detect Is Grounded
        // Check the Gizmos to see what the OverlapBox looks like
        Vector3 sizeBox = transform.localScale / 2;
        sizeBox.y = m_GroundedOffset / 2;
        List<Collider> r = Physics.OverlapBox(transform.position + (Vector3.down * (transform.localScale.y + m_GroundedOffset + 0.01f) / 2), sizeBox).ToList();
        _isGrounded = 0 != r.Count;
        #endregion

        if (HDirection != 0)
        {
            FlipFacingDirection();
        }
    }

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;

        transform.rotation = Quaternion.Euler(0, HDirection < 0 ? 0 : 180, 0);
    }

    protected void SetGrid(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    protected void Jump(float jumpMultiplier = 1f)
    {
        if (!_isGrounded) return;

        _rb.AddForce(_jumpForce * jumpMultiplier * Vector3.up, ForceMode.Impulse);
    }

    //protected void JumpUp()

    protected void Move(float movementMultiplier = 1f)
    {
        float targetSpeed = _horizontal * _speed * movementMultiplier;
        float speedDif = targetSpeed - _rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? m_Acceleration : m_Decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, m_VelocityPower) * Mathf.Sign(speedDif);
        //Vector3 force =  * Time.fixedDeltaTime * new Vector3(_horizontal, 0f, 0f);
        _rb.AddForce(movement * Vector3.right);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Vector3 leftRay = new(transform.position.x - transform.localScale.x /2, transform.position.y - transform.localScale.y / 2 - m_GroundedOffset, transform.position.z);
        //Vector3 rightRay = leftRay + Vector3.right * transform.localScale.x;
        //Gizmos.DrawLine(leftRay, rightRay);
        Vector3 sizeBox = transform.localScale / 2;
        sizeBox.y = m_GroundedOffset/2;
        Gizmos.DrawWireCube(transform.position + (Vector3.down * (transform.localScale.y + m_GroundedOffset + 0.01f) / 2), sizeBox * 2);
    }
}
