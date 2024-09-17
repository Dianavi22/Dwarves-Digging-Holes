using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private Transform objectSlot;
    [SerializeField] private Collider PickaxeCollider;
    private bool isCatch = false;



    void Start()
    {

    }

    public void OnCatch(InputAction.CallbackContext context)
    {
        isCatch = context.action.triggered;
        isCatch = false;

    }

    void Update()
    {
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Pickaxe"))
        {
            CatchPickaxe();
           // collision.collider.gameObject.transform = objectSlot.transform;
        }
    }

    public void CatchPickaxe()
    {
        print("HERE");
    }
}
