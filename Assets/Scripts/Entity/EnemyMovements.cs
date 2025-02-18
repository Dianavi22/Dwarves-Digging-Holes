using UnityEngine;
using System;
using UnityEngine.InputSystem;
using FMODUnity;
using Utils;
using System.Collections.Generic;

public class EnemyMovements : EntityMovement
{
    [SerializeField] private EventReference disappointedSound;
    [SerializeField] GameObject raycastDetectHitWall;
    [SerializeField] GameObject raycastDetectHitCeiling;
    [SerializeField] ParticleSystem _angryEnemy;
    [SerializeField] float distanceToWall;
    [SerializeField] float degToWall;
    [SerializeField] LayerMask _goblinLayer;
    Enemy _e => (Enemy)GetBase;

    private bool hitCeiling = false;
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
        if (GameManager.Instance.isGameOver) return;

        if (!_e.IsGrabbed)
        {
            base.Update();

            List<Collider> hits = DRayCast.Cone(raycastDetectHitWall.transform.position, -transform.right, degToWall, distanceToWall, 3, ~_goblinLayer);
            hitWall = hits.Count > 0;
            hitCeiling = Physics.Raycast(raycastDetectHitCeiling.transform.position, transform.up, 1.5f);

            //Debug.DrawRay(raycastDetectHitWall.transform.position, -transform.right * 1.5f, Color.red);
            Debug.DrawRay(raycastDetectHitCeiling.transform.position, transform.up * 1.5f, Color.red);

            if (hitWall && !hitCeiling)
            {
                if (isGrounded && !_e.IsTouchingChariot)
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

            OnMove();

            //lost Gold function
            if (_e.IsTouchingChariot && !_e.IsGrabbed && _e.canSteal)
            {
                _e._animator.SetTrigger("stealGold");
                StartCoroutine(_e.HitChariot());
            }
        }

        _angryEnemy.transform.rotation = Quaternion.Euler(-90, 0, transform.rotation.y == -180 ? 180 : 0);

        if (_e.IsGrabbed && !_e.isDead)
        {
            if (!_angryEnemy.isPlaying)
            {
                _angryEnemy.Play();
                DisappointedSound();
            }
        }
        else
        {
            _angryEnemy.Stop();
        }

        if (!_e.IsTouchingChariot)
        {
            _e._animator.ResetTrigger("stealGold");
        }
    }

    private bool IsInCorner()
    {
        return hitWall && hitCeiling;
    }

    private void OnMove()
    {
        float _horizontal = Mathf.Sign(_e.GetDestinationPosition.x - transform.position.x);
        if (Math.Abs(_e.GetDestinationPosition.x - transform.position.x) <= 1.25f || IsInCorner())
            _horizontal = 0f;

        _e._animator.SetFloat("run", _horizontal);
        CanMove = !_e.IsTouchingChariot || IsInCorner();
        Move(_horizontal * Vector2.right);
    }

    #region Sound
    private void DisappointedSound()
    {
        RuntimeManager.PlayOneShot(disappointedSound, transform.position);
    }

    #endregion
}
