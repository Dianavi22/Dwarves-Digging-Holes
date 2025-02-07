using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] GameObject _pickaxeCount;
    [SerializeField] GameObject _tutoEnemy;
    [SerializeField] GameObject _panelTuto;
    [SerializeField] TMP_Text _panelTxtTuto;
    [SerializeField] Image _panelImageTuto;
    [SerializeField] List<Sprite> _panelImageListTuto;


    [SerializeField] ParticleSystem _transitionPart;
    [SerializeField] ParticleSystem _endTutoPart;
    [SerializeField] Animator _panelAnimator;
    [SerializeField] ShakyCame _sc;
    [SerializeField] TypeSentence _ts;

    private bool _isInCdLava = false;
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
                _panelTuto.SetActive(true);
                _panelImageTuto.sprite = _panelImageListTuto[0];
                if (_panelTxtTuto.text != "Take Pickaxe")
                {
                    Invoke("PlayPart", 0.5f);
                }
                _panelTxtTuto.text = "Take Pickaxe";
                TakePickaxe();
            }

            if (isBreakRock)
            {
                BreakRock();
                _panelImageTuto.sprite = _panelImageListTuto[1];
                if (_panelTxtTuto.text != "Break Rock")
                {
                    _transitionPart.Play();
                    _panelAnimator.SetTrigger("ChangeTuto");
                }
                _panelTxtTuto.text = "Break Rock";
            }

            if (isPushChariot)
            {
                PushChariot();
                _panelImageTuto.sprite = _panelImageListTuto[0];
                if (_panelTxtTuto.text != "Push chariot")
                {
                    _transitionPart.Play();
                    _panelAnimator.SetTrigger("ChangeTuto");


                }
                _panelTxtTuto.text = "Push chariot";

            }

            if (isTakeEnemy)
            {
                TakeEnemy();
                _panelImageTuto.sprite = _panelImageListTuto[0];
                if (_panelTxtTuto.text != "Take Enemy")
                {
                    _transitionPart.Play();
                    _panelAnimator.SetTrigger("ChangeTuto");

                }
                _panelTxtTuto.text = "Take Enemy";

            }

            if (isYeetEnemy)
            {
                YeetEnemy();
                _panelImageTuto.sprite = _panelImageListTuto[0];
                if (_panelTxtTuto.text != "Yeet Enemy")
                {
                    _transitionPart.Play();
                    _panelAnimator.SetTrigger("ChangeTuto");

                }
                _panelTxtTuto.text = "Yeet Enemy";

            }
        }


    }

    private void PlayPart()
    {
        _transitionPart.Play();
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
        if (!_isInCdLava)
        {
            _isInCdLava = true;
            StartCoroutine(TargetManager.Instance.GetGameObject<Lava>().CooldownLava());

        }
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
        _panelAnimator.SetTrigger("EndTuto");
        Invoke("EndTutoPart", 0.7f);
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

    private void EndTutoPart()
    {
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _endTutoPart.Play();
        _pickaxeCount.SetActive(true);
    }

}
