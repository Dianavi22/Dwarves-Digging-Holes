using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float speedModifier = 1;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Physics.SyncTransforms();
        Vector3 goalDestination = GameManager.Instance.Difficulty.ScrollingSpeed * speedModifier * Time.fixedDeltaTime * Vector3.left;
        _rb.MovePosition(transform.position + goalDestination);
    }
}
