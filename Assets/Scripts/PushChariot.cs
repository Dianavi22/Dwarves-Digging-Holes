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
        if (_isTriggerActive)
            _rb.AddForce(pushForce, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.name == "GFX")
        {
            if (!other.transform.parent.parent.GetComponent<PlayerActions>().isHoldingObject)
            {
                _script.enabled = false;
                var platformScript = other.transform.parent.parent.GetComponent<Platform>();
                if (platformScript) platformScript.enabled = false;
                _isTriggerActive = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.name == "GFX")
        {
            _isTriggerActive = false;
            _script.enabled = true;
            var platformScript = other.transform.parent.parent.GetComponent<Platform>();
            if (platformScript) platformScript.enabled = true;
        }
    }
}
