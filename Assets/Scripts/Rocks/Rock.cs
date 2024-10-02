using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public int _healthPoint = 5;

    [SerializeField]
    private bool _haveGold;
    private GoldChariot _goldChariot;
    [SerializeField] ParticleSystem _rockHitPart;
    [SerializeField] ParticleSystem _rockGoldPart;

    private Collider _rockCollider;
    [SerializeField] private GameObject _gfx;

    private void Awake()
    {
        if (_haveGold) _goldChariot = TargetManager.Instance.GetGameObject(Target.GoldChariot).GetComponent<GoldChariot>();
       
        _rockCollider = this.GetComponentInChildren<Collider>();
    
    }

    public void Hit()
    {
        _healthPoint -= 1;

        if (!_haveGold)
        {
            _rockHitPart.Play();

        }
        else
        {
            _rockGoldPart.Play();
        }

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
            _goldChariot.GoldCount += 1;
        }

        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        Invoke("DestroyGameObject", 1f);
    }
    private void DestroyGameObject()
    {
        Destroy(gameObject);

    }


}
