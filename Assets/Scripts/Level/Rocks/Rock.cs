using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class Rock : MonoBehaviour
{
    [SerializeField] int _healthPoint = 5;
    public bool haveGold;
    [SerializeField] ParticleSystem _breakRockParticule;
    private Collider _rockCollider;
    [SerializeField] private GameObject _gfx;
    private Score _score;
    [SerializeField] private int _goldScore;
    [SerializeField] Transform _spawnGold;
    [SerializeField] GameObject _gold;
    [SerializeField] EventManager _eventManager;
    private int _baseHp;
    private Player hitPlayer = null;

    private void Awake()
    {
        _rockCollider = GetComponentInChildren<Collider>();
        _eventManager = FindObjectOfType<EventManager>();
        _spawnGold = this.transform;
    }

    private void Start()
    {
        if (!GameManager.Instance.isInMainMenu)
        {
            _score = TargetManager.Instance.GetGameObject<Score>();
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
        Tuto tuto = TargetManager.Instance.GetGameObject<Tuto>();
        if (tuto.isBreakRock)
        {
            tuto.isPushChariot = true;
        }
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.1f, 0.1f);
        if (haveGold)
        {
            TargetManager.Instance.GetGameObject<GoldChariot>().GoldCount += 1;
            _score.ScoreCounter += _goldScore;
            Instantiate(_gold, new Vector3(_spawnGold.position.x, _spawnGold.position.y, 0), Quaternion.identity);
            if (hitPlayer != null) StatsManager.Instance.IncrementStatistic(hitPlayer, StatsName.GoldMined, 5);

        }

        _breakRockParticule.Play();
        _gfx.SetActive(false);
        _rockCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!GameManager.Instance.isInMainMenu)
        {
            if (_eventManager.isRockEvent)
            {
                EventRock();
            }
            else
            {
                _healthPoint = _baseHp;
            }
        }

    }

    private void EventRock()
    {
        _baseHp = _healthPoint;
        _healthPoint += 1;
    }
}
