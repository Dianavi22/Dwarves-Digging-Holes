using System;
using System.Collections;
using UnityEngine;

public class Enemy : EntityMovement, IGrabbable
{
    [SerializeField] float lifePoint = 3f;

    [SerializeField] GameObject raycastDetectHitWall;

    private Vector3 mvtVelocity;

    private GoldChariot _goldChariot;
    private bool _isTouchChariot;
    private bool IsTouchingChariot 
    { 
        get => _isTouchChariot;
        set
        {
            if (_isTouchChariot == value) return;

            if (value)
                _goldChariot.NbGoblin++;
            else
                _goldChariot.NbGoblin--;
            _isTouchChariot = value;
        }
    }
    private bool _InCD = false;

    // if the entity can change his focus from the primary target (like by targeting the player if one damaged him)
    private bool hasFocus = true;
    private bool isGrabbed;

    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        this.isGrabbed = isGrabbed;
        if (isGrabbed)
        {
            hasFocus = false;
        } else
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0));
        }
    }

    void Start()
    {
        SetRigidbody(GetComponent<Rigidbody>());
        _goldChariot = TargetManager.Instance.GetGameObject(Target.GoldChariot).GetComponent<GoldChariot>();
    }

    void FixedUpdate()
    {
        // Can jump part
        bool hitWall = Physics.Raycast(raycastDetectHitWall.transform.position, transform.forward, 1.5f);

        // Grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (hitWall) Jump();
        //else
        //{
        //    mvtVelocity.y = -2f;
        //    mvtVelocity.y += gravityValue * Time.deltaTime;
        //    _rb.AddForce(mvtVelocity * Time.deltaTime);
        //}

        Vector3 goldChariotPosition = _goldChariot.transform.position;
        float direction = Math.Sign(goldChariotPosition.x - transform.position.x);

        float offset = 1f;
        if (goldChariotPosition.x - offset < transform.position.x && goldChariotPosition.x + offset > transform.position.x)
            direction = 0f;

        SetGrid(direction, 0f);

        // to make sure the enemy isn't stuck in a wall while jumping we stop its movement
        if (!hitWall && hasFocus) Move();

        if (!hasFocus && !isGrabbed) hasFocus = isGrounded;
        
        if (isGrabbed)
        {
            IsTouchingChariot = false;
            //_rb.mass = 1f;
        }
        //else
        //{
        //    _rb.mass = 10f;
        //}
    }

    private new void Update()
    {
        base.Update();
        //lost Gold function
        if (IsTouchingChariot && !_InCD && !isGrabbed)
        {
            StartCoroutine(HitChariot());
        }
    }

    private IEnumerator HitChariot()
    {
        _InCD = true;
        _goldChariot.GoldCount -= 1;
        yield return new WaitForSeconds(1);
        _InCD = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            Debug.Log("---- GoldChariot collision");
            IsTouchingChariot = true;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            Debug.Log("---- Quitting");
            IsTouchingChariot = false;
        }
    }
}
