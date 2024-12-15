using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tuto : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Lava _lava;
    [SerializeField] GoldChariot _goldChariot;
    [SerializeField] GameManager _gameManager;
    [SerializeField] Pickaxe _takePickaxe;

    [Header("Bubbles")]
    [SerializeField] GameObject _breakRock;
    [SerializeField] GameObject _pushChariot;
    [SerializeField] GameObject _takeEnemy;
    [SerializeField] GameObject _bubbleLava;
    [SerializeField] GameObject _tutoBubbleLava;
    [SerializeField] GameObject _skipTuto;

    [Header("Circles")]
    [SerializeField] GameObject _circleLava;
    [SerializeField] GameObject _wallLimitTuto;

    [HideInInspector] public bool startTuto;
    [HideInInspector] public bool isBreakRock;
    [HideInInspector] public bool isPushChariot;
    [HideInInspector] public bool isTakeEnemy;
    [HideInInspector] public bool isYeetEnemy = false;
    [HideInInspector] public bool isInTuto = false;

    [SerializeField] GameObject _tutoEnemy;

    void Start()
    {
        _takePickaxe = FindFirstObjectByType<Pickaxe>();
    }

    void Update()
    {
        if (startTuto)
        {
            isInTuto = true;
            TakePickaxe();
        }

        if (isBreakRock)
        {
            BreakRock();
        }

        if (isPushChariot)
        {
            PushChariot();
        }

        if (isTakeEnemy)
        {
            TakeEnemy();
        }

        if (isYeetEnemy)
        {
            YeetEnemy();
        }
    }

    private void TakePickaxe()
    {
        _takePickaxe.isInTuto = true;
    }
    private void BreakRock()
    {
        _takePickaxe.isInTuto = false;
        startTuto = false;
        _breakRock.SetActive(true);
    }

    private void PushChariot()
    {
        _goldChariot.GetComponent<GoldChariot>().enabled = true;
        isBreakRock = startTuto;
        try
        {
            _breakRock.SetActive(startTuto);
        }
        catch
        {
            //
        }
        _pushChariot.SetActive(true);

    }

    private void TakeEnemy()
    {
        isPushChariot = false;
        _pushChariot.SetActive(isPushChariot);
        _tutoEnemy.SetActive(true);
        _takeEnemy.SetActive(true);
    }

    private void YeetEnemy()
    {
        isTakeEnemy = false;
        _takeEnemy.SetActive(isTakeEnemy);
        StartCoroutine(_lava.CooldownLava());
        _wallLimitTuto.SetActive(false);
        _circleLava.SetActive(true);
        _tutoBubbleLava.SetActive(true);
    }

    public void StopTuto()
    {
        _takePickaxe.isInTuto = false;
        _bubbleLava.SetActive(false);
        _circleLava.SetActive(false);

        startTuto = false;
        isBreakRock = false;
        isPushChariot = false;
        isTakeEnemy = false;
        isYeetEnemy = false;

        try
        {
            _breakRock.SetActive(false);
        }
        catch
        {
            //
        }
        _pushChariot.SetActive(false);
        _takeEnemy.SetActive(false);
        _wallLimitTuto.SetActive(false);
        _tutoBubbleLava.SetActive(false);
        isInTuto = false;
        _skipTuto.SetActive(false);
        StartCoroutine(_gameManager.StartGame());
    }

}
