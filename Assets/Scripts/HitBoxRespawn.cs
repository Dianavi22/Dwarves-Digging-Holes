using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxRespawn : MonoBehaviour
{
    public bool isReadyToRespawn;

    void Start()
    {
        
    }

    void Update()
    {
        var colliders = Physics.OverlapSphere(new Vector3(this.transform.position.x, this.transform.position.y, 0), 1);
        foreach (var collider in colliders)
        {
            if(colliders.Length != 0)
            {
                isReadyToRespawn = false;

            }
            
        }

        if (colliders.Length == 0)
        {
            isReadyToRespawn = true;

        }
       
    }
}
