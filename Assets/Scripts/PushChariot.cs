using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushChariot : MonoBehaviour
{
    [SerializeField] float pushForce = 150;
    private bool _isTriggerActive;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTriggerActive)
            _rb.AddForce(pushForce, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other " + other);
        if (other.CompareTag("Player") && other.name == "GFX" && !other.transform.root.GetComponent<PlayerActions>().isHoldingObject)
        {
            _isTriggerActive = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.name == "GFX")
        {
            _isTriggerActive = false;
        }
    }
}
