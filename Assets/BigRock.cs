using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRock : MonoBehaviour
{
   

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Dynamite>(collision.collider, out var dynamite))
        {
            Destroy(dynamite.gameObject);
        }
    }
}
