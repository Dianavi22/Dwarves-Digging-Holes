using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class EnemyMovements : EntityMovement
{
    [SerializeField] GameObject raycastDetectHitWall;
    Enemy _e => (Enemy)GetBase;

    void Awake()
    {
        GetBase = GetComponent<Enemy>();
    }

    private void Start()
    {
        SetStats(GameManager.Instance.Difficulty.GoblinStats);
    }

    protected new void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            _e.KillGobs();
            return;
        }
        if (!_e.IsGrabbed)
        {
            base.Update();

            OnMove();

            bool hitWall = Physics.Raycast(raycastDetectHitWall.transform.position, -transform.right, 1.5f) || Physics.Raycast(raycastDetectHitWall.transform.position, transform.forward, 1.5f);

            if (hitWall)
            {
                if (isGrounded)
                {
                    IsPerformingJump = true;
                    Jump();
                }
            }
            // If it also hit a wall while jumping, it will do a long jump
            else if((!hitWall || GetBase.GetRigidbody().velocity.y < 0) && IsPerformingJump)
            {
                IsPerformingJump = false;
            }

            //lost Gold function
            if (_e.IsTouchingChariot && !_e.IsGrabbed && _e.canSteal)
            {
                StartCoroutine(_e.HitChariot());
                //_e.transform.position += new Vector3(horizontalInput / 2.25f, 0, 0f) * Time.deltaTime;
            }
        }
    }
    private void OnMove()
    {
        Vector3 goldChariotPosition = _e._goldChariot.transform.position;
        float _horizontal = Mathf.Sign(goldChariotPosition.x - transform.position.x);
        if (Math.Abs(Vector3.Distance(goldChariotPosition, transform.position)) <= 1.25f)
            _horizontal = 0f;

        CanMove = !_e.IsTouchingChariot;
        Move(_horizontal * Vector2.right);
    }
}
