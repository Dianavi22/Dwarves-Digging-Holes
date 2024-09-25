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

    public static T GetParentComponent<T>(GameObject _gameObject)
    {
        return _gameObject.transform.root.GetComponent<T>();
    }
    public static bool TryGetParentComponent<T>(GameObject _gameObject, out T _out)
    {
        bool t = _gameObject.transform.root.TryGetComponent(out T _out2);
        _out = _out2;
        return t;
    }

    public static T GetParentComponent<T>(Collider _gameObject)
    {
        return _gameObject.transform.root.GetComponent<T>();
    }
    public static bool TryGetParentComponent<T>(Collider _gameObject, out T _out)
    {
        bool t = _gameObject.transform.root.TryGetComponent(out T _out2);
        _out = _out2;
        return t;
    }
}
