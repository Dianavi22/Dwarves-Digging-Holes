using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Pickaxe : MonoBehaviour, IGrabbable
{
    [SerializeField] private EventReference mineSoundEvent;
    [SerializeField] private EventReference swingSoundEvent;


    [SerializeField] ParticleSystem _hitRockParts;
    [SerializeField] ParticleSystem _hitGoldParts;
    [SerializeField] ParticleSystem _hitPickaxe;
    private bool _isPartPlayed = true;
    private bool _isDying = false;
    private Player holdingPlayer;
    public bool isInTuto;
    [SerializeField] FollowTarget _tutoTarget;
    public FollowTarget myTarget;
    [SerializeField] GameObject _pickaxePart;
    private bool _isShowTuto = false;
    private bool _isCarried = false;

    // In case the set of HealthPoint want to destroy the pickaxe
    // _healthPoint is update in GameManager
    private int _healthPoint = 1;
    public int HealthPoint
    {
        get => _healthPoint;
        set
        {
            _healthPoint = value;
            if (_healthPoint <= 0 && !_isDying)
                // StartCoroutine(BreakPickaxe());
                return;
        }
    }

    private bool isFirstTime = true;

    private Action throwOnDestroy;
    private void Start()
    {
        StartCoroutine(CdParticule());
        myTarget = Instantiate(_tutoTarget, transform.position, Quaternion.identity);
        myTarget.target = transform;
    }

    private void Update()
    {
        if (!_isDying)
        {
            if (isInTuto || _isShowTuto && GameManager.Instance.isGameStarted)
            {
                myTarget.OpenTuto();
            }
            else
            {
                if (isFirstTime)
                {
                    isFirstTime = false;
                    myTarget.TotalClose();

                }
                else
                {
                    myTarget.CloseTuto();
                }
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isPartPlayed)
        {
            _isPartPlayed = true;
            _hitPickaxe.Play();
            StartCoroutine(CdParticule());
        }
    }

    private IEnumerator CdParticule()
    {
        yield return new WaitForSeconds(1);
        _isPartPlayed = false;
    }

    #region Hit
    public void Hit(GameObject hit)
    {
        if (Utils.Component.TryGetInParent<Rock>(hit, out var rock))
        {
            HandleRockHit(rock);

        }

        else if (Utils.Component.TryGetInParent<Player>(hit, out var player))
        {
            HandlePlayerHit(player);
        }
    }

    private void HandleRockHit(Rock rock)
    {
        rock.Hit(holdingPlayer);

        MineSound();

        if (rock.haveGold)
        {
            _hitGoldParts.Play();
        }
        else
        {
            _hitRockParts.Play();
        }
        HealthPoint--;
    }

    private void HandlePlayerHit(Player player)
    {
        player.GetActions().ForceDetach();
        player.GetHealth().Stun();
    }
    #endregion

    #region IGrabbable
    public void HandleCarriedState(Player currentPlayer, bool isCarried)
    {
        holdingPlayer = isCarried ? currentPlayer : null;
        PlayerActions actions = currentPlayer.GetActions();
        currentPlayer.GetAnimator().SetBool("hasPickaxe", isCarried);
        _isCarried = isCarried;
        if (isCarried)
        {
            throwOnDestroy = () =>
            {
                holdingPlayer = null;
                actions.EmptyHands();
                actions.StopAnimation();
                currentPlayer.GetAnimator().SetBool("hasPickaxe", false);
                actions.IsBaseActionActivated = false;
            };
            // Reset pickaxe scale when throw to avoid scaling issues
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (!isInTuto && GameManager.Instance.isGameStarted)
            {
                _isShowTuto = false;
            }
        }
        else
        {
            actions.StopAnimation();
            actions.IsBaseActionActivated = false;
            if (!isInTuto)
            {
                StartCoroutine(TutoInGame());
            }
        }
    }

    public void HandleDestroy()
    {
        StartCoroutine(BreakPickaxe());
    }

    public GameObject GetGameObject() => gameObject;
    #endregion

    private void OnDestroy()
    {
        GameManager.Instance.NbPickaxe--;
        throwOnDestroy?.Invoke();
    }

    private IEnumerator BreakPickaxe()
    {
        _isDying = true;
        Destroy(myTarget.gameObject);
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.2f, 0.2f);
        Destroy(gameObject);
        GameObject myBreakPart = Instantiate(_pickaxePart, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(myBreakPart);
    }
    private IEnumerator TutoInGame()
    {
        yield return new WaitForSeconds(3);
        if (!_isCarried)
        {
            _isShowTuto = true;
        }
        else
        {
            yield return null;
        }
    }

    #region Sound
    private void MineSound()
    {
        EventInstance miningSoundInstance = RuntimeManager.CreateInstance(mineSoundEvent);
        RuntimeManager.AttachInstanceToGameObject(miningSoundInstance, transform, GetComponent<Rigidbody>());
        miningSoundInstance.start();
        miningSoundInstance.release();
    }

    #endregion
}
