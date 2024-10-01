using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushChariot : MonoBehaviour
{
    [SerializeField] float pushForce = 150;
    private bool _isTriggerActive;
    private Rigidbody _rb;
    private PlayerActions _playerActions;
    private PlayerFatigue _playerFatigue;
    private bool _isCartPushing, _hasFatigue;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTriggerActive && _playerFatigue.ReduceCartsFatigue(10f * Time.deltaTime))
        {
            _rb.AddForce(pushForce, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out var player) && !player.isHoldingObject)
        {
            _isTriggerActive = true;
            _playerFatigue = player.playerFatigue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out _))
        {
            _isTriggerActive = false;
            _playerFatigue = null;
        }
    }
}
