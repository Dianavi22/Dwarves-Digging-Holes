using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuild : MonoBehaviour
{

    private bool _isNearBeerChariot;
    void Start()
    {
        
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.GetComponent<BeerChariot>() && _isNearBeerChariot) 
        {
            PrepareBeer();
        }
    }

    private void PrepareBeer()
    {
        print("PREPARE BEER");
    }

}
