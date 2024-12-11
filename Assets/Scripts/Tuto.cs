using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public bool startTuto; 
    public bool isBreakRock;
    public bool isPushChariot;
    public bool isTakeEnemy;
    public bool isYeetEnemy;
    void Start()
    {
        _takePickaxe = FindFirstObjectByType<Pickaxe>();
    }

    void Update()
    {
        if (startTuto)
        {
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
        Invoke("ActiveCircle", 2.5f);
    }

    private void ActiveCircle() { 
        _circleLava.SetActive(true);
    }

    public void StopTuto()
    {
        _bubbleLava.SetActive(false);
        _circleLava.SetActive(false);
    }

}
