using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

public class Enemy : MonoBehaviour
{
    [SerializeField] Target primaryTarget;

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float lifePoint = 3f;
    [SerializeField] float jumpForce = 0.5f;

    // if the entity can change his focus from the primary target (like by targeting the player if one damaged him)
    public bool hasFocus = true;
    public bool isGrabbed;

    [SerializeField] GameObject raycastDetectHitWall;

    private Vector3 mvtVelocity;
    private Rigidbody _rb;
    private bool flip = false;

    private readonly float gravityValue = -9.81f;

    private GoldChariot _goldChariot;
    private bool _isTouchingChariot = false;
    private bool _InCD = false;

   [SerializeField] private ParticleSystem _goldOutChariot;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Can jump part
        bool hitWall = Physics.Raycast(raycastDetectHitWall.transform.position, transform.forward, 1.5f);
        // Grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (hitWall && isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            mvtVelocity.y = -2f;
            mvtVelocity.y += gravityValue * Time.deltaTime;
            _rb.AddForce(mvtVelocity * Time.deltaTime);
        }

        Transform currentTarget = TargetManager.Instance.GetGameObject(primaryTarget).transform;
        float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);

        if (direction == -1f && flip || direction == 1f && !flip)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        float offset = 1f;
        if (currentTarget.position.x - offset < transform.position.x && currentTarget.position.x + offset > transform.position.x)
            direction = 0f;

        // to make sure the enemy isn't stuck in a wall while jumping we stop its movement
        if (!hitWall && hasFocus) _rb.velocity = new Vector3(movementSpeed * direction, _rb.velocity.y, 0f);

        // TODO faire condition isGrabbed
        if (!hasFocus && !isGrabbed) hasFocus = isGrounded;
        
        //lost Gold function
        if (_isTouchingChariot && !_InCD)
        {
            StartCoroutine(HitChariot());
        }
        if (isGrabbed)
        {
            _rb.mass = 1f;
        }
        else {
            _rb.mass = 10f;

        }



    }
    private void FlipFacingDirection()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0,-1.1f,0));
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
        if (Utils.TryGetParentComponent<GoldChariot>(collision.collider, out var goldChariot) && goldChariot.gameObject.name == "GoldChariot")
        {
            _goldChariot = goldChariot;
            _isTouchingChariot = true;
            _goldOutChariot = _goldChariot.GetComponentInChildren<ParticleSystem>();
            _goldOutChariot.Play();
        }
    }
    
    private void OnCollisionExit(Collision collision) { 
    
        _isTouchingChariot = false;
        if (_goldOutChariot != null)
        {
            _goldOutChariot.Stop();
            _goldOutChariot = null;

        }
    }

}
