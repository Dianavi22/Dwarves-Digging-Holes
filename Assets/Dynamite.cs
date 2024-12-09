using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.collider.GetComponentInParent<GameObject>().name == "BigRock")
            {
                Destroy(collision.collider);
            }
        }
        catch
        {
            //
        }
       
    }
}
