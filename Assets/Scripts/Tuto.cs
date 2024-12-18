using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tuto : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Pickaxe _takePickaxe;

    [Header("Bubbles")]
    [SerializeField] GameObject _breakRock;
    [SerializeField] GameObject _pushChariot;
    [SerializeField] GameObject _takeEnemy;
    [SerializeField] GameObject _tutoBubbleLava;
    [SerializeField] GameObject _skipTuto;

    [HideInInspector] public bool startTuto;
    [HideInInspector] public bool isBreakRock;
    [HideInInspector] public bool isPushChariot;
    [HideInInspector] public bool isTakeEnemy;
    [HideInInspector] public bool isYeetEnemy = false;
    [HideInInspector] public bool isInTuto = false;

    [SerializeField] GameObject _wallLimitTuto;
    [SerializeField] GameObject _tutoEnemy;

    void Start()
    {
        _takePickaxe = FindFirstObjectByType<Pickaxe>();
    }

    void Update()
    {
        if (isInTuto)
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
      
    }

    private void TakePickaxe()
    {
        _takePickaxe.isInTuto = true;
    }
    private void BreakRock()
    {
        _takePickaxe.isInTuto = false;
        startTuto = false;
        _breakRock.GetComponent<FollowTarget>().OpenTuto();
    }

    private void PushChariot()
    {
        TargetManager.Instance.GetGameObject<GoldChariot>().enabled = true;
        isBreakRock = startTuto;
        try
        {
            _breakRock.GetComponent<FollowTarget>().CloseTuto();
        }
        catch
        {
            //
        }
        _pushChariot.GetComponent<FollowTarget>().OpenTuto();

    }

    private void TakeEnemy()
    {
        isPushChariot = false;
        _pushChariot.GetComponent<FollowTarget>().CloseTuto();
        _tutoEnemy.SetActive(true);
        _takeEnemy.GetComponent<FollowTarget>().OpenTuto();
    }

    private void YeetEnemy()
    {
        isTakeEnemy = false;
        _takeEnemy.SetActive(isTakeEnemy);
        StartCoroutine(TargetManager.Instance.GetGameObject<Lava>().CooldownLava());
        _wallLimitTuto.SetActive(false);
        _tutoBubbleLava.GetComponent<FollowTarget>().OpenTuto();
    }

    public void StopTuto()
    {
        _takePickaxe.isInTuto = false;
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
        _pushChariot.GetComponent<FollowTarget>().CloseTuto();
        _takeEnemy.GetComponent<FollowTarget>().CloseTuto();
        _wallLimitTuto.SetActive(false);
        _tutoBubbleLava.GetComponent<FollowTarget>().CloseTuto();
        isInTuto = false;
        _skipTuto.SetActive(false);
        StartCoroutine(GameManager.Instance.StartGame());
    }

}
