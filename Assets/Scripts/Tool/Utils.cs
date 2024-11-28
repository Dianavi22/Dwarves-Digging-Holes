using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static readonly string BEST_SCORE_KEY = "BEST_SCORE";

    #region Get Component

    /**
     * @deprecated
     */
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

    public static T GetGameObjectComponent<T>(GameObject _gameObject)
    {
        TryGetParentComponent(_gameObject, out T _out);
        return _out;
    }
    public static bool TryGetParentComponent<T>(GameObject _gameObject, out T _out)
    {
        if (_gameObject.TryGetComponent(out T _out2))
        {
            _out = _out2;
            return true;
        }
        else if (_gameObject.transform.parent != null)
            return TryGetParentComponent(_gameObject.transform.parent.gameObject, out _out);
        else {
            _out = default;
            return false;
        }
    }

    public static T GetParentComponent<T>(Collider _gameObject)
    {
        TryGetParentComponent(_gameObject, out T _out);
        return _out;
    }
    public static bool TryGetParentComponent<T>(Collider _gameObject, out T _out)
    {
        return TryGetParentComponent(_gameObject.gameObject, out _out);
    }

    public static bool TryGetChildComponent<T>(GameObject _gameObject, out T _out, int index)
    {
        if (_gameObject.TryGetComponent(out T _out2))
        {
            _out = _out2;
            return true;
        }
        else if (_gameObject.transform.childCount >= index - 1)
            return TryGetChildComponent(_gameObject.transform.GetChild(index).gameObject, out _out, index);
        else {
            _out = default;
            return false;
        }
    }
    #endregion

    public static List<Collider> ConeRayCast(Vector3 origin, Vector3 direction, float angle, float maxDistance, int numRays, LayerMask layerHit)
    {
        List<Collider> allhits = new();
        for (int i = 0; i < numRays; i++)
        {
            float currentAngle = Mathf.Lerp(-angle / 2, angle / 2, i / (float)(numRays - 1));
            Vector3 rayDirection = Quaternion.Euler(0, 0, currentAngle) * direction;

            if (Physics.Raycast(origin, rayDirection, out RaycastHit hit, maxDistance, layerHit))
            {
                //Debug.Log(hit.collider.gameObject.name);
                Debug.DrawRay(origin, rayDirection * hit.distance, Color.green, 0.1f);
                allhits.Add(hit.collider);
            }
            else
            {
                Debug.DrawRay(origin, rayDirection * maxDistance, Color.red, 0.1f);
            }
        }
        return allhits;
    }
}
