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
    
    public static readonly string BEST_SCORE_KEY = "BEST_SCORE";
}
