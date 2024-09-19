using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private int _healthPoint = 5;

    [SerializeField]
    private bool _haveGold;

    public void Hit()
    {
        _healthPoint -= 1;

        if (_healthPoint <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        if (_haveGold)
        {
            Debug.Log("GOLD");
        }

        Destroy(gameObject);
    }
}
