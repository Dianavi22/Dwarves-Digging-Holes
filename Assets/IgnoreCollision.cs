using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    private TheEndObj _theEnd;
    [SerializeField] private Collider _collider;
    void Start()
    {
        _theEnd = FindObjectOfType<TheEndObj>();
    }

    void Update()
    {
        if (_theEnd.isDwarfUp && !_collider.isTrigger)
        {
            print("Collision");
            _collider.isTrigger = true;
        }

    }

  
}
