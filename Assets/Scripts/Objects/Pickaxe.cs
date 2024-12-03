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
    [SerializeField] ShakyCame _sc;
    private bool _isPartPlayed = true;
    private bool _isDying = false;

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
                StartCoroutine(BreakPickaxe());
               
        }
    }

    private Action throwOnDestroy;
    private void Start()
    {
        StartCoroutine(CdParticule());
        _sc = FindObjectOfType<ShakyCame>();
    }
    public void HandleCarriedState(Player currentPlayer, bool isCarried)
    {
        PlayerActions actions = currentPlayer.GetActions();
        currentPlayer.GetAnimator().SetBool("hasPickaxe", isCarried);

        if (isCarried)
        {
            throwOnDestroy = () => { 
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
        if (Utils.TryGetParentComponent<Rock>(hit, out var rock))
        {
            HandleRockHit(rock);
            
        }
        
        else if (Utils.TryGetParentComponent<Player>(hit, out var player))
        {
            HandlePlayerHit(player);
        }
    }

    private void HandleRockHit(Rock rock)
    {
        rock.Hit();
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

    public GameObject GetGameObject() { return gameObject; }

    private void OnDestroy()
    {
        GameManager.Instance.NbPickaxe--;
        throwOnDestroy?.Invoke();
    }

    private IEnumerator BreakPickaxe() {
        _isDying = true;
        print("DestroyPickaxe");
        _breakPickaxe.Play();
        _sc.ShakyCameCustom(0.2f, 0.2f);
        _gfx.SetActive(false);
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }

}