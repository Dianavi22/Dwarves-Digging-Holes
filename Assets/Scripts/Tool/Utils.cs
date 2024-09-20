using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static GameObject GetCollisionGameObject(Collider hitCollider)
    {
        try
        {
            return hitCollider.gameObject.transform.parent.gameObject;
        }
        catch (System.Exception)
        {
           return hitCollider.gameObject;
        }
    }
}
