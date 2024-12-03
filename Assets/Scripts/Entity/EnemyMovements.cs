using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections;

public class EnemyMovements : EntityMovement
{

    Enemy GetBase => (Enemy)_p;
    private bool canJump = true;

    void Awake()
    {
        _p = GetComponent<Enemy>();
    }

    override protected void Update()
    {
        if (GameManager.Instance.isGameOver) GetBase.KillGobs();
        if (!_p.IsGrabbed)
        {
            base.Update();

            bool hitWall = Physics.Raycast(GetBase.raycastDetectHitWall.transform.position, transform.forward, 1.5f);

            if (hitWall && isGrounded && canJump)
            {
                SetCanJump();
                Invoke(nameof(SetCanJump), 0.25f);
                Jump();
            }


            //lost Gold function
            if (GetBase.IsTouchingChariot && !_p.IsGrabbed && GetBase.canSteal)
            {
                StartCoroutine(GetBase.HitChariot());
            }
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

    private void SetCanJump() {
        canJump = !canJump;
    }
}
