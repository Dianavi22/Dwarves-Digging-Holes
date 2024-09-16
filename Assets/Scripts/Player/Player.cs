using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float _horizontal = 0f;
    private Rigidbody _rb;

    [SerializeField]
    private float _speed = 8f;

    private float _distanceToGround;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PrintAllGamepads();
    }
    private void PrintAllGamepads()
    {
        var gamepads = Gamepad.all;
        Debug.Log($"Number of connected gamepads: {gamepads.Count}");
        
        foreach (var gamepad in gamepads)
        {
            Debug.Log($"Gamepad: {gamepad.displayName}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position, -Vector3.up, out hit))
        { 
            _distanceToGround = hit.distance;
        }

    }
    void OnMove(InputValue value) {
        _horizontal = value.Get<Vector2>().x;
    }

    void OnJump() {
        if(_distanceToGround <= 1f) {
            _rb.AddForce(_rb.velocity.x, 10f, 0f, ForceMode.Impulse);
        }
    }
}
