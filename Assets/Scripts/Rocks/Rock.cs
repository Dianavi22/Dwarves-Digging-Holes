using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] int _healthPoint = 5;
    public bool haveGold;
    [SerializeField] ParticleSystem _breakRockParticule;
    private Collider _rockCollider;
    [SerializeField] private GameObject _gfx;
    [SerializeField] ShakyCame _shakyCame;

    private void Awake()
    {       
        _rockCollider = GetComponentInChildren<Collider>();
        _shakyCame = FindObjectOfType<ShakyCame>();
    }

    public void Hit()
    {
        _healthPoint -= 1;
        if (_healthPoint <= 0)
            Break();
    }

    public void Break()
    {
        _shakyCame._radius = 0.1f;
        _shakyCame._duration = 0.1f;
        _shakyCame.isShaking = true;
        if (haveGold)
        {
            TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot).GoldCount += 1;
        }
        
            _breakRockParticule.gameObject.SetActive(true);
        
        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        Destroy(gameObject);
    }
}
