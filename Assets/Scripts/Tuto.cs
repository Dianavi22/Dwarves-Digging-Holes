using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tuto : MonoBehaviour
{

    [SerializeField] Pickaxe _takePickaxe;
    [SerializeField] GameObject _breakRock;
    [SerializeField] GameObject _pushChariot;
    [SerializeField] GameObject _takeEnemy;
    [SerializeField] GameObject _tutoEnemy;
    [SerializeField] GameObject _bubbleLava;
    [SerializeField] Lava _lava;
    [SerializeField] GoldChariot _goldChariot;

    [SerializeField] GameObject _circleRocks;
    [SerializeField] GameObject _circleChariot;
    [SerializeField] GameObject _circleEnemy;
    [SerializeField] GameObject _circleLava;
    [SerializeField] GameObject _wallLimitTuto;
    [SerializeField] GameObject _tutoBubbleLava;

    [SerializeField] GameManager _gameManager;


    public bool startTuto;
    public bool isBreakRock;
    public bool isPushChariot;
    public bool isTakeEnemy;
    public bool isYeetEnemy = false;
    public bool isInTuto = false;
    [SerializeField] GameObject _skipTuto;
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
        _circleRocks.SetActive(true);
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
        _circleRocks.SetActive(startTuto);
        _circleChariot.SetActive(true);

    }

    private void TakeEnemy()
    {
        isPushChariot = false;
        _pushChariot.SetActive(isPushChariot);
        _tutoEnemy.SetActive(true);
        _takeEnemy.SetActive(true);
        _circleChariot.SetActive(isPushChariot);
        _circleEnemy.SetActive(true);
    }

    private void YeetEnemy()
    {
        isTakeEnemy = false;
        _takeEnemy.SetActive(isTakeEnemy);
        _circleEnemy.SetActive(isTakeEnemy);
        StartCoroutine(_lava.CooldownLava());
        _wallLimitTuto.SetActive(false);
        _circleLava.SetActive(true);
        _tutoBubbleLava.SetActive(true);

    }

    private void ActiveCircle()
    {
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
        _circleRocks.SetActive(false);
        _circleChariot.SetActive(false);
        _circleEnemy.SetActive(false);
        _wallLimitTuto.SetActive(false);
        _tutoBubbleLava.SetActive(false);
        isInTuto = false;
        _skipTuto.SetActive(false);
        StartCoroutine(_gameManager.StartGame());
    }

}
