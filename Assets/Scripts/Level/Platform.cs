using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float blockDifficulty;
    private float _speedModifier = 1;
    private Rigidbody _rb;
    [SerializeField] GameManager _gameManager;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        Physics.SyncTransforms();
        Vector3 goalDestination = GameManager.Instance.Difficulty.ScrollingSpeed * _speedModifier * Time.fixedDeltaTime * Vector3.left;
        _rb.MovePosition(transform.position + goalDestination);
        if (_gameManager.isGameOver)
        {
            _speedModifier = 0;
        }
    }
}
