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
            if (!collider.isTrigger)
            {
                isReadyToRespawn = false;
            }
            else
            {
                isReadyToRespawn = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(this.transform.position.x, this.transform.position.y, 0), 1);
    }
}
