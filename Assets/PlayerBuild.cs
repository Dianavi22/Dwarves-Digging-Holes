using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuild : MonoBehaviour
{

    private bool _isNearBeerChariot; 
    private float requiredHoldTime = 3f;
    private PlayerActions _playerActions;
    [SerializeField] private GameObject _beerPrefab;
    void Start()
    {
        _playerActions = this.GetComponent<PlayerActions>();
    }

    void Update()
    {
        
    }

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isNearBeerChariot = true;

        }
        else
        {
            _isNearBeerChariot = false;
        }

        if (context.phase == InputActionPhase.Performed)
        {
            StartCoroutine(BuildingWaiting());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<BeerChariot>() && _isNearBeerChariot) 
        {
            PrepareBeer();
        }
    }
    private IEnumerator BuildingWaiting()
    {
        yield return new WaitForSeconds(requiredHoldTime);
        PrepareBeer();
    }


    private void PrepareBeer()
    {
        _playerActions.currentHeldObject = _beerPrefab;
        _playerActions.isHoldingObject = true;
        _playerActions.heldObject = Instantiate(_beerPrefab, _playerActions.objectSlot);
    }

}
