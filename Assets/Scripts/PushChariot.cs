using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushChariot : MonoBehaviour
{
    [SerializeField] float pushForce = 150;
    private bool _isTriggerActive;
    private Rigidbody _rb;

    private Platform _script;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _script = GetComponent<Platform>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isPushed = _isTriggerActive;
        if (isPushed)
            _rb.AddForce(pushForce, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            _isTriggerActive = true;
            _script.enabled = false;
            other.GetComponent<Platform>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = false;
            _script.enabled = true;
            other.GetComponent<Platform>().enabled = true;
        }
    }
}
