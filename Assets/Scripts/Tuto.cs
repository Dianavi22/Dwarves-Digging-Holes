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
    [SerializeField] Lava _lava;

    [SerializeField] GameObject _circleRocks;
    [SerializeField] GameObject _circleChariot;
    [SerializeField] GameObject _circleEnemy;
    [SerializeField] GameObject _circleLava;


    public bool startTuto; 
    public bool _isBreakRock;
    public bool _isPushChariot;
    public bool _isTakeEnemy;
    public bool _isYeetEnemy;
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

        if (_isBreakRock)
        {
            BreakRock();
        }

        if (_isPushChariot)
        {
            PushChariot();
        }
        
        if (_isTakeEnemy)
        {
            TakeEnemy();
        }
       
        if (_isYeetEnemy)
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
        _isBreakRock = startTuto;
        _breakRock.SetActive(startTuto);
        _pushChariot.SetActive(true);
        _circleRocks.SetActive(startTuto);
        _circleChariot.SetActive(true);

    }

    private void TakeEnemy()
    {
        _isPushChariot = false;
        _pushChariot.SetActive(_isPushChariot);
        _tutoEnemy.SetActive(true);
        _takeEnemy.SetActive(true);
        _circleChariot.SetActive(_isPushChariot);
        _circleEnemy.SetActive(true);
    }

    private void YeetEnemy()
    {
        _isTakeEnemy = false;
        _takeEnemy.SetActive(_isTakeEnemy);
        _circleEnemy.SetActive(_isTakeEnemy);
        _circleLava.SetActive(true);
        StartCoroutine(_lava.CooldownLava());
    }


}
