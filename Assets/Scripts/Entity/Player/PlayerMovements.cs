using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using FMODUnity;

public class PlayerMovements : EntityMovement
{
    [HideInInspector] public bool isGrabbingChariot;
    
    [SerializeField] private Vector2 _deadZoneSpace = new(0.5f, 0.5f);

    [Header("Particle effect")]
    [SerializeField] ParticleSystem _DashPart;
    [SerializeField] ParticleSystem _hurtPart;
    [SerializeField] ParticleSystem _groundedPart;
    [SerializeField] ParticleSystem _movePart;
    [SerializeField] ParticleSystem _tearsPart;

    [SerializeField] private EventReference dashSound;
    [SerializeField] private EventReference jumpSound;
    [SerializeField] private EventReference landingSound;
    [SerializeField] private EventReference disappointedgSound;
    
    
    private bool _isDashingCooldown = false;
    private bool _isDashing = false;
    private bool flip_vertical = false;
    [SerializeField] Rigidbody _rb;

    public Action forceDetachFunction;

    Player _p => (Player)GetBase;


    void Awake()
    {
        GetBase = GetComponent<Player>();
    }

    protected new void Update()
    {
        if (_p.IsGrabbed && !_tearsPart.isPlaying)
        {
            _tearsPart.Play();
            DisappointedSound(gameObject.transform.position);
        }

        if (!_p.IsGrabbed)
        {
            _tearsPart.Stop();
        }

        base.Update();

        if ((_moveInput.y != 0 && !flip_vertical) || (_moveInput.y == 0 && flip_vertical))
        {
            FlipHoldObject();
        }
    }

    private bool PlayerCanMove(bool isInputActivated)
    {
        if (!GameManager.Instance.isGameOver)
        {
            bool isHoldingChariot = _p.HasJoint && Utils.Component.TryGetInParent<GoldChariot>(_p.GetActions().heldObject, out _);
            if (isInputActivated && isHoldingChariot)
                return !_p.IsGrabbed && _p.GetFatigue().ReduceCartsFatigue(
                    GameManager.Instance.Difficulty.PushCartFatigue.ActionReducer * Time.deltaTime);
        }

        return !_p.IsGrabbed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Enemy>(collision.collider, out var enemy) && _isDashing)
        {
            enemy.HandleDestroy();
        }

        if (!collision.collider.CompareTag("GoldChariot") && !_groundedPart.isPlaying)
        {
            _groundedPart.Play();
            LandingSound(gameObject.transform.position);
        }
    }

    private void FlipFacingDirection()
    {
        if (GameManager.Instance.isGameOver) return;
        transform.rotation = Quaternion.Euler(0, flip ? 0 : 180, 0);
    }

    private void FlipHoldObject()
    {
        float targetZRotation = -Math.Sign(_moveInput.y) * 35f;

        if (_p.GetActions().pivot.transform.localEulerAngles.z == targetZRotation) return;

        //_p.GetActions().StopAnimation();
        //_p.GetActions().CancelInvoke();
        _p.GetActions().pivot.transform.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0f);
        _p.GetModelRef().m_HeadAimTarget.transform.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0f);
        _p.GetActions().vertical = _moveInput.y;
        flip_vertical = _moveInput.y != 0;
    }

    #region EVENTS
    public void OnMove(InputAction.CallbackContext context)
    {
       // Debug.Log(CanDoAnything());
        if (!_p.CanDoAnything()) return;

        Vector2 vector = context.ReadValue<Vector2>();
        float _horizontal = Mathf.Abs(vector.x) > _deadZoneSpace.x ? vector.x : 0;
        float _vertical = Mathf.Abs(vector.y) > _deadZoneSpace.y ? Mathf.RoundToInt(vector.y) : 0;
        if (_horizontal != 0 && !_movePart.isPlaying)
        {
            _movePart.Play();
        }

        if (_horizontal == 0)
        {
            _movePart.Stop();
        }
        CanMove = PlayerCanMove(Mathf.Abs(_horizontal) > 0);
        //Debug.Log(CanMove);
        Move(new Vector2(_horizontal, _vertical));

        // Grab chariot moonwalk fix
        float horizontal = (isGrabbingChariot && flip) ? _horizontal : _horizontal * -1f;
        _p.GetAnimator().SetFloat("Run", horizontal);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;
        
        if (_p.IsGrabbed)
        {
            forceDetachFunction?.Invoke();
        }
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (isGrounded && !_isDashing)
                {
                    IsPerformingJump = true;
                    Jump();
                    JumpSound(gameObject.transform.position);
                    _movePart.Stop();
                }
                break;
            case InputActionPhase.Canceled:
                IsPerformingJump = false;
                break;
        }
    }
    public void OnDash(InputAction.CallbackContext _)
    {
       if(!_DashPart.isPlaying) _DashPart.Play();
        if (!_p.CanDoAnything()) return;

        if (_isDashing || _isDashingCooldown || _p.IsGrabbed) return;

        _isDashing = true;
        _isDashingCooldown = true;
        _DashPart.Play();
        DashSound(gameObject.transform.position);

        Dash();

        DOVirtual.DelayedCall(0.2f, () =>
        {
            _DashPart.Stop();
            _isDashing = false;
            Invoke(nameof(EndDashCoolDown), 0.8f);
            _DashPart.Stop();
        });
    }
    #endregion

    // Fin du cooldown du dash
    void EndDashCoolDown()
    {
        _isDashingCooldown = false;
    }

    private bool _isOnChariot = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HitGoldByChariot>(out var hgbc) && !_isOnChariot)
        {
            _isOnChariot = true;
            hgbc.HitByPlayer(other.transform.position);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isOnChariot = false;
    }


    #region Sound

    private void DashSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(dashSound, position);
    }

    private void JumpSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(jumpSound, position);
    }

    private void LandingSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(landingSound, position);
    }

        private void DisappointedSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(disappointedgSound, position);
    }

    #endregion

}
