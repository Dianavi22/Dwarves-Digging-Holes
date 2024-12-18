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
    [SerializeField] GameObject _gfx;
    private bool _isPartPlayed = true;
    private bool _isDying = false;
    private Player holdingPlayer;
    public bool isInTuto;
    [SerializeField] GameObject _tutoTarget;
    [SerializeField] GoldChariot _gc;
    public GameObject myTarget;
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

    private Action throwOnDestroy;
    private void Start()
    {
        StartCoroutine(CdParticule());
        myTarget = Instantiate(_tutoTarget, transform.position, Quaternion.identity);
        myTarget.GetComponent<FollowTarget>().target = transform;
        _gc = FindObjectOfType<GoldChariot>();
    }
    public void HandleCarriedState(Player currentPlayer, bool isCarried)
    {
        holdingPlayer = isCarried ? currentPlayer : null;
        PlayerActions actions = currentPlayer.GetActions();
        currentPlayer.GetAnimator().SetBool("hasPickaxe", isCarried);
        if (isCarried)
        {
            _isCarried = true;
            throwOnDestroy = () => { 
                holdingPlayer = null;
                actions.EmptyHands();
                actions.StopAnimation();
                currentPlayer.GetAnimator().SetBool("hasPickaxe", false);
                actions.IsBaseActionActivated = false;
            };
            if (!isInTuto && GameManager.Instance.isGameStarted)
            {
                _isShowTuto = false;
            }
        }
        else
        {
            _isCarried = false;
            actions.StopAnimation();
            actions.IsBaseActionActivated = false;
            if (!isInTuto)
            {
                StartCoroutine(TutoInGame());
            }
        }
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

    private void Update()
    {
        if(isInTuto || _isShowTuto && GameManager.Instance.isGameStarted)
        {
            print("is in tuto");
            myTarget.GetComponent<FollowTarget>().OpenTuto();
        }
        else {
            myTarget.GetComponent<FollowTarget>().CloseTuto();
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

    public void HandleDestroy()
    {
        StartCoroutine(BreakPickaxe());
    }

    public GameObject GetGameObject() => gameObject;

    private void OnDestroy()
    {
        GameManager.Instance.NbPickaxe--;
        throwOnDestroy?.Invoke();
    }

    private IEnumerator BreakPickaxe() {
        _isDying = true;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.2f, 0.2f);
        _gfx.SetActive(false);
        GameObject myBreakPart = Instantiate(_pickaxePart, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(myBreakPart);
        Destroy(this.gameObject);
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
