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
    [SerializeField] Score _score;
    [SerializeField] private int _goldScore;
    [SerializeField] Transform _spawnGold;
    [SerializeField] GameObject _gold;
    [SerializeField] Tuto _tuto;

    private void Awake()
    {       
        _rockCollider = GetComponentInChildren<Collider>();
        _score = FindObjectOfType<Score>();
        _tuto = FindObjectOfType<Tuto>();
    }

    public void Hit()
    {
        _healthPoint -= 1;
        if (_healthPoint <= 0)
            StartCoroutine(Break());
    }

    public IEnumerator Break()
    {

        if (_tuto.isBreakRock)
        {
            _tuto.isPushChariot = true;
        }
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.1f, 0.1f);
        if (haveGold)
        {
            TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot).GoldCount += 1;
            _score.ScoreCounter += _goldScore;
            Instantiate(_gold, new Vector3(_spawnGold.position.x, _spawnGold.position.y, 0), Quaternion.identity);
        }

        _breakRockParticule.Play();
        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
