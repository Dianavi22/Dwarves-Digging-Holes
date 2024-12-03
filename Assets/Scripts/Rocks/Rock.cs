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
    [SerializeField] Score _score;

    private void Awake()
    {       
        _rockCollider = GetComponentInChildren<Collider>();
        _shakyCame = FindObjectOfType<ShakyCame>();
        _score = FindObjectOfType<Score>();
    }

    public void Hit()
    {
        _healthPoint -= 1;
        if (_healthPoint <= 0)
            StartCoroutine(Break());
    }

    public IEnumerator Break()
    {
        _shakyCame.ShakyCameCustom(0.1f, 0.1f);
        if (haveGold)
        {
            TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot).GoldCount += 1;
            _score.AddScoreOnce(10);

        }

        _breakRockParticule.Play();
        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
