using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject primaryTarget;

    [SerializeField] float movementSpeed = 5f;

    // if the entity can change his focus from the primary target (like by targeting the player if one damaged him)
    [SerializeField] bool canChangeFocus;
    [SerializeField] bool grabbable;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 mvtVelocity;
    private Rigidbody _rb;

    private readonly float gravityValue = -9.81f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform currentTarget = primaryTarget.transform;
        float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
        float offset = 1f;
        if (currentTarget.position.x-offset < transform.position.x && currentTarget.position.x+offset > transform.position.x) 
            direction = 0f;

        //Debug.Log(direction);
        _rb.velocity = new Vector3(movementSpeed * direction, _rb.velocity.y, 0f);

        // Grounded
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        if (!isGrounded)
        {
            mvtVelocity.y = -2f;
            mvtVelocity.y += gravityValue * Time.deltaTime;
            _rb.AddForce(mvtVelocity * Time.deltaTime);
        }
    }
}
