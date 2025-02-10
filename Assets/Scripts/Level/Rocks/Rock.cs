using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using FMODUnity;
using FMOD.Studio;
using Random = UnityEngine.Random;

public class Rock : MonoBehaviour
{
    [SerializeField] private EventReference rockExplosionSound;

    [SerializeField] int _healthPoint = 5;
    public bool haveGold;
    [SerializeField] ParticleSystem _breakRockParticule;
    [SerializeField] ParticleSystem _spawnGoldPart;
    [SerializeField] ParticleSystem _destroyRockByLava;
    private Collider _rockCollider;
    [SerializeField] private GameObject _gfx;
    [SerializeField] private int _goldScore;
    [SerializeField] Transform _spawnGold;
    [SerializeField] GameObject _gold;
    private int _baseHp;
    private Player hitPlayer = null;
    [SerializeField] Material _goldMat;

    private void Awake()
    {
        _rockCollider = GetComponentInChildren<Collider>();
        _spawnGold = this.transform;
    }

    private void Start()
    {
        if (haveGold)
        {
            _destroyRockByLava.GetComponent<Renderer>().material = _goldMat;
        }
        else
        {
            _destroyRockByLava.GetComponent<Renderer>().material = gameObject.GetComponentInChildren<Renderer>().material;
        }
    }

    public void Hit(Player player)
    {
        if (hitPlayer != player) hitPlayer = player;

        _healthPoint -= 1;
        if (_healthPoint <= 0)
            StartCoroutine(Break());
    }

    public IEnumerator Break()
    {
        _breakRockParticule.Play();
        _gfx.SetActive(false);
        _rockCollider.enabled = false;

        Tuto tuto = TargetManager.Instance.GetGameObject<Tuto>();
        if (tuto.isBreakRock)
        {
            tuto.isPushChariot = true;
        }
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.1f, 0.1f);
        if (haveGold)
        {
            _spawnGoldPart.Play();
            if (!GameManager.Instance.isInMainMenu) StartCoroutine(BreakGold());
        }

        PlayRockExplosionSound(gameObject.transform.position);

        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public IEnumerator BreakGold()
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(_gold, new Vector3(_spawnGold.position.x, _spawnGold.position.y, 0), Quaternion.identity);
        if (hitPlayer != null) StatsManager.Instance.IncrementStatistic(hitPlayer, StatsName.GoldMined, 1);
    }

    private void PlayRockExplosionSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(rockExplosionSound, position);
    }

    public IEnumerator DestroyRockByLava()
    {
        _gfx.SetActive(false);
        _destroyRockByLava.Play();
        yield return new WaitForSeconds(_destroyRockByLava.main.duration);
        Destroy(this.gameObject);
    }
}
