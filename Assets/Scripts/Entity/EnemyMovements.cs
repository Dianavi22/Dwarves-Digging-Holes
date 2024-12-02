using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections;

public class EnemyMovements : EntityMovement
{

    new private Enemy _p;

    override protected void Awake()
    {
        base.Awake();
        _p = GetComponent<Enemy>();
    }

    override protected void Update()
    {
        if (GameManager.Instance.isGameOver) return;
        if (!_p.IsGrabbed)
        {
            Vector3 goldChariotPosition = _p._goldChariot.transform.position;
            horizontalInput = Mathf.Sign(goldChariotPosition.x - transform.position.x);

            HandleGround();

            bool hitWall = Physics.Raycast(_p.raycastDetectHitWall.transform.position, transform.forward, 1.5f);

            Debug.Log(hitWall);

            if (hitWall && isGrounded)
            {
                Jump();
            }

            HandleFlip();

            if (Math.Abs(Vector3.Distance(goldChariotPosition, transform.position)) <= 1.25f)
                horizontalInput = 0f;

            if (_p.IsTouchingChariot)
            {
                _p.transform.position += new Vector3(horizontalInput / 2.25f, 0, 0f) * Time.deltaTime;
            }
            else
            {
                _p.GetRigidbody().velocity = new Vector3(speed * horizontalInput, _p.GetRigidbody().velocity.y, 0f);
            }


            //lost Gold function
            if (_p.IsTouchingChariot && !_p.IsGrabbed && _p.canSteal)
            {
                StartCoroutine(_p.HitChariot());
            }

            HandleJumpPhysics();
        }
    }
}
