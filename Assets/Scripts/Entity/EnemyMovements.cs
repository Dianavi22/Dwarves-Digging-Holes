using UnityEngine;
using System;

public class EnemyMovements : EntityMovement
{
    [SerializeField] GameObject raycastDetectHitWall;
    Enemy _e => (Enemy)GetBase;
    private bool canJump = true;

    private bool hitWall = false;

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

            if (isGrounded)
            {
                hitWall = Physics.Raycast(raycastDetectHitWall.transform.position, -transform.right, 1.5f) || Physics.Raycast(raycastDetectHitWall.transform.position, transform.forward, 1.5f);
                if (hitWall && canJump)
                {
                    SetCanJump();
                    Invoke(nameof(SetCanJump), 0.25f);
                    Jump();
                }
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
        Move(_horizontal);
    }

    private void SetCanJump() {
        canJump = !canJump;
    }
}
