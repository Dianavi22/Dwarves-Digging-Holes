using UnityEngine;
using System;

public class EnemyMovements : EntityMovement
{

    Enemy GetBase => (Enemy)_p;
    private Transform _goldChariot;

    void Awake()
    {
        _p = GetComponent<Enemy>();
        _goldChariot = GetBase._goldChariot.transform;
    }
    override protected void Update()
    {
        if (GameManager.Instance.isGameOver) return;
        if (!_p.IsGrabbed)
        {
            base.Update();
            
            bool hitWall = Physics.Raycast(GetBase.raycastDetectHitWall.transform.position, transform.forward, 1.5f);

            if (hitWall && isGrounded)
            {
                Jump();
            }
            //HandleMovement();


            //lost Gold function
            if (GetBase.IsTouchingChariot && !GetBase.IsGrabbed && GetBase.canSteal)
            {
                StartCoroutine(GetBase.HitChariot());
            }
        }
    }

    override protected void HandleMovement()
    {
        horizontalInput = Mathf.Sign(_goldChariot.position.x - transform.position.x);
        if (Math.Abs(Vector3.Distance(_goldChariot.position, transform.position)) <= 1.25f)
            horizontalInput = 0f;

        if (GetBase.IsTouchingChariot)
        {
            _p.transform.position += new Vector3(horizontalInput / 2.25f, 0, 0f) * Time.deltaTime;
        }
        else
        {
            _p.GetRigidbody().velocity = new Vector3(speed * horizontalInput, GetBase.GetRigidbody().velocity.y, 0f);
        }
    }
}
