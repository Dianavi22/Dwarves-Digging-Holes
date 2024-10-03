using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] int _healthPoint = 5;
    [SerializeField] bool _haveGold;

    [SerializeField] ParticleSystem _hitRockParticule;

    private Collider _rockCollider;
    [SerializeField] private GameObject _gfx;

    private void Awake()
    {       
        _rockCollider = GetComponentInChildren<Collider>();
    }

    public void Hit()
    {
        _healthPoint -= 1;

        _hitRockParticule.Play();

        if (_healthPoint <= 0)
            Break();
    }

    public void Break()
    {
        if (_haveGold)
            TargetManager.Instance.GetGameObject(Target.GoldChariot).GetComponent<GoldChariot>().GoldCount += 1;

        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        Destroy(gameObject);
    }
}
