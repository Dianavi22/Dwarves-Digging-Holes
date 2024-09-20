using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float speedModifier = 1;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        Physics.SyncTransforms();
        float movementSpeed = transform.position.x - Time.deltaTime * GameManager.Instance.scrollingSpeed * speedModifier;
        _rb.MovePosition(new Vector3(movementSpeed, transform.position.y, transform.position.z));
    }
}
