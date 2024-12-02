using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IGrabbable
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float lifePoint = 3f;
    [SerializeField] float jumpForce = 0.5f;

    [SerializeField] GameObject raycastDetectHitWall;
    [SerializeField] GameObject _gfx;

    private Vector3 mvtVelocity;
    private Rigidbody _rb;
    private bool flip = false;

    [SerializeField] ParticleSystem _destroyGobPart;
    [SerializeField] ShakyCame _shakyCame;

    [SerializeField] GameManager _gameManager;

    // The Physics Gravity is changed, so we set a new one for the enemy
    private readonly float gravityValue = -9.81f;

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
    public bool hasFocus = true;
    public bool isGrabbed;

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
    
    public void HandleDestroy()
    {
        StartCoroutine(DestroyByLava());

    }

    public GameObject GetGameObject() { return gameObject; }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _rb = GetComponent<Rigidbody>();
        _shakyCame = FindObjectOfType<ShakyCame>();
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
    }

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

        Vector3 goldChariotPosition = _goldChariot.transform.position;
        float direction = Mathf.Sign(goldChariotPosition.x - transform.position.x);

        if (direction == -1f && flip || direction == 1f && !flip)
        {
            flip = !flip;
            FlipFacingDirection();
        }

        float offset = 1f;
        if (goldChariotPosition.x - offset < transform.position.x && goldChariotPosition.x + offset > transform.position.x)
            direction = 0f;

        // to make sure the enemy isn't stuck in a wall while jumping we stop its movement
        if (!hitWall && hasFocus) _rb.velocity = new Vector3(movementSpeed * direction, _rb.velocity.y, 0f);

        // TODO faire condition isGrabbed
        if (!hasFocus && !isGrabbed) hasFocus = isGrounded;
        
        //lost Gold function
        if (IsTouchingChariot && !_InCD && !isGrabbed)
        {
            StartCoroutine(HitChariot());
        }

        if (isGrabbed)
        {
            IsTouchingChariot = false;
            _rb.mass = 1f;
        }
        else 
        {
            _rb.mass = 10f;
        }

        if (_gameManager.isGameOver)
        {
            KillGobs();
        }
    }
    private void FlipFacingDirection()
    {
        transform.Rotate(0f, 180f, 0f);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -1.1f, 0));
    }

    private void KillGobs()
    {
        _gfx.SetActive(false);
        _rb.velocity = Vector3.zero;
    }

    public IEnumerator DestroyByLava()
    {
        _rb.velocity = Vector3.zero;
        _shakyCame.ShakyCameCustom(0.3f, 0.3f);
        _gfx.SetActive(false);
        _destroyGobPart.Play();
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
