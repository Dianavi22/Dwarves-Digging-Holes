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
        print("TakePickaxe");
    }
    private void BreakRock()
    {
        _takePickaxe.isInTuto = false;
        startTuto = false;
        _breakRock.SetActive(true);
    }

    private void PushChariot()
    {
        _isBreakRock = false;
        _breakRock.SetActive(false);
        _pushChariot.SetActive(true);
    }

    private void TakeEnemy()
    {
        _isPushChariot = false;
        _pushChariot.SetActive(false);
        _tutoEnemy.SetActive(true);
        _takeEnemy.SetActive(true);
    }

    private void YeetEnemy()
    {
        _isTakeEnemy = false;
        _takeEnemy.SetActive(false);
        StartCoroutine(_lava.CooldownLava());
    }


}
