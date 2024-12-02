using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections;

public class EnemyMovements : EntityMovement
{

    Enemy GetBase => (Enemy)_p;

    override protected void Awake()
    {
        base.Awake();
        _p = GetComponent<Enemy>();
    }

    override protected void Update()
    {
        if (GameManager.Instance.isGameOver) GetBase.KillGobs();
        if (!_p.IsGrabbed)
        {
            HandleGround();

            bool hitWall = Physics.Raycast(GetBase.raycastDetectHitWall.transform.position, transform.forward, 1.5f);

            if (hitWall && isGrounded)
            {
                Jump();
            }

            HandleFlip();

            HandleMovement();


            //lost Gold function
            if (GetBase.IsTouchingChariot && !_p.IsGrabbed && GetBase.canSteal)
            {
                StartCoroutine(GetBase.HitChariot());
            }

            HandleJumpPhysics();
        }
    }
    override protected void HandleMovement()
    {
        Vector3 goldChariotPosition = GetBase._goldChariot.transform.position;
        horizontalInput = Mathf.Sign(goldChariotPosition.x - transform.position.x);
        if (Math.Abs(Vector3.Distance(goldChariotPosition, transform.position)) <= 1.25f)
            horizontalInput = 0f;

        if (GetBase.IsTouchingChariot)
        {
            _p.transform.position += new Vector3(horizontalInput / 2.25f, 0, 0f) * Time.deltaTime;
        }
        else
        {
            _p.GetRigidbody().velocity = new Vector3(speed * horizontalInput, _p.GetRigidbody().velocity.y, 0f);
        }
    }
}
