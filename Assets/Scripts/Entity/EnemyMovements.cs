using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class EnemyMovements : EntityMovement
{
    [SerializeField] GameObject raycastDetectHitWall;
    [SerializeField] ParticleSystem _angryEnemy;
    [SerializeField] Animator _animator;
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
        if (GameManager.Instance.isGameOver) return;

        if (!_e.IsGrabbed)
        {
            base.Update();

            OnMove();

            bool hitWall = Physics.Raycast(raycastDetectHitWall.transform.position, -transform.right, 1.5f) || Physics.Raycast(raycastDetectHitWall.transform.position, transform.forward, 1.5f);
            //Debug.Log(hitWall);

            Debug.DrawRay(raycastDetectHitWall.transform.position, -transform.right * 1.5f, Color.red);
            Debug.DrawRay(raycastDetectHitWall.transform.position, transform.right * 1.5f, Color.red);

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
                _animator.SetTrigger("stealGold");
                StartCoroutine(_e.HitChariot());
            }
        }

        if (this.transform.rotation.y == -180)
        {
            _angryEnemy.transform.rotation = Quaternion.Euler(-90, 0, 180);
        }
        else
        {
            _angryEnemy.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        if (_e.IsGrabbed && !_e.isDead)
        {
            if (!_angryEnemy.isPlaying)
            {
                _angryEnemy.Play();
            }
        }
        else
        {
            _angryEnemy.Stop();
        }
    }
    private void OnMove()
    {
        float _horizontal = Mathf.Sign(_e.GetDestinationPosition.x - transform.position.x);
        if (Math.Abs(Vector3.Distance(_e.GetDestinationPosition, transform.position)) <= 1.25f)
            _horizontal = 0f;

        _animator.SetFloat("run", _horizontal);
        CanMove = !_e.IsTouchingChariot;
        Move(_horizontal * Vector2.right);
    }
}
