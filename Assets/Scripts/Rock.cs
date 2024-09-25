using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public int _healthPoint = 5;

    [SerializeField]
    private bool _haveGold;
    private GoldChariot _goldChariot;

    private void Awake()
    {
        if (_haveGold) _goldChariot = TargetManager.Instance.GetGameObject(Target.GoldChariot).GetComponent<GoldChariot>();
    }

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
            _goldChariot.addGold();
        }

        Destroy(gameObject);
    }
}
