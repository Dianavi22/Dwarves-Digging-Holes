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
        if (Utils.TryGetParentComponent<PlayerActions>(other, out var player) && !player.isHoldingObject)
        {
            _isTriggerActive = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out _))
        {
            _isTriggerActive = false;
        }
    }
}
