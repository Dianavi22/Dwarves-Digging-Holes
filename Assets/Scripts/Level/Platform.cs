using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float speedModifier = 1;
    private float movementSpeed;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        movementSpeed = GameManager.Instance.scrollingSpeed;
    }

    private void Update()
    {
        Physics.SyncTransforms();
        _rb.MovePosition(new Vector3(transform.position.x - Time.deltaTime * movementSpeed * speedModifier, transform.position.y, transform.position.z));
    }
}
