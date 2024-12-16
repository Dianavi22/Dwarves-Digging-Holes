using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (Utils.Component.TryGetInParent<Dynamite>(collision.collider, out var dynamite))
        {
            Physics.IgnoreCollision(dynamite.GetComponentInChildren<Collider>(), GetComponent<Collider>());
        }

    }
}
