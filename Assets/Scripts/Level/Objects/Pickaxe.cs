using System;
using System.Collections;
using UnityEngine;

public class Pickaxe : MonoBehaviour, IGrabbable
{
    [SerializeField] ParticleSystem _hitRockParts;
    [SerializeField] ParticleSystem _hitGoldParts;
    [SerializeField] ParticleSystem _hitPickaxe;
    [SerializeField] ParticleSystem _breakPickaxe;
    [SerializeField] GameObject _gfx;
    private bool _isPartPlayed = true;
    private bool _isDying = false;
    private Player holdingPlayer;
    public bool isInTuto;
    [SerializeField] GameObject _tutoTarget;
    [SerializeField] GoldChariot _gc;
    public GameObject myTarget;

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
            throwOnDestroy = () => { 
                holdingPlayer = null;
                actions.EmptyHands();
                actions.StopAnimation();
                currentPlayer.GetAnimator().SetBool("hasPickaxe", false);
                actions.IsBaseActionActivated = false;
            };
        }
        else
        {
            actions.StopAnimation();
            actions.IsBaseActionActivated = false;
        }
    }

    private void Update()
    {
        myTarget.SetActive(isInTuto);
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
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.2f, 0.2f);
        _gfx.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        _breakPickaxe.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}