using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushChariot : MonoBehaviour
{
    [SerializeField] float pushForce = 85;
    private bool _isTriggerActive;
    private Rigidbody _rb;
    private float _initMass;
    private Vector3 _initPos;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _initMass = _rb.mass;
        _initPos = transform.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool isPushed = _isTriggerActive && Gamepad.current.leftTrigger.isPressed;
        if (isPushed) _rb.mass = _initMass - pushForce;
        else _rb.mass = _initMass;

        transform.position = new Vector3(transform.position.x, transform.position.y, _initPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = false;
        }
    }
}
