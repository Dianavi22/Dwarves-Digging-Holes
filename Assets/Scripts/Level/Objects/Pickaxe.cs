using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Pickaxe : MonoBehaviour, IGrabbable
{
    [SerializeField] private EventReference mineSoundEvent;
    [SerializeField] private EventReference pickaxeThrowSound;

    [SerializeField] ParticleSystem _hitRockParts;
    public ParticleSystem _hitGoldParts;
    [SerializeField] ParticleSystem _hitPickaxe;
    [SerializeField] ParticleSystem _spawnPickaxePart;
    [SerializeField] ParticleSystem _pickaxePart;
    private bool _isPartPlayed = true;
    private bool _isDying = false;
    private Player holdingPlayer;
    public bool isInTuto;
    [SerializeField] GameObject _GFX;
    [SerializeField] FollowTarget _tutoTarget;
    private bool _isShowTuto = false;
    private bool _isCarried = false;

    public FollowTarget myTarget;

    private bool isFirstTime = true;

    private Action throwOnDestroy;
    
    private void Start()
    {
        StartCoroutine(CdParticule());
        myTarget = Instantiate(_tutoTarget, transform.position, Quaternion.identity);
        myTarget.target = transform;
        myTarget.gameObject.SetActive(!GameManager.Instance.isInMainMenu);
        if (!GameManager.Instance.isInMainMenu)
        {
            GameManager.Instance.nbPickaxeUI.text = (Int32.Parse(GameManager.Instance.nbPickaxeUI.text) + 1).ToString();
        }
        _spawnPickaxePart.Play();
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
            if (!_isCarried) transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
            if (rock.haveGold)
            {
                PlayPart();
            }
            else
            {
                _hitRockParts.Play();
            }
            HandleRockHit(rock);

        }

        else if (Utils.Component.TryGetInParent<Player>(hit, out var player))
        {
            HandlePlayerHit(player);
        }
    }

    private void HandleRockHit(Rock rock)
    {
        rock.Hit(holdingPlayer, this);

        MineSound();
       
    }

    private void PlayPart()
    {
        _hitGoldParts.Play();
    }

    private void HandlePlayerHit(Player player)
    {
        player.GetActions().ForceDetach();
        player.GetHealth().Stun();
    }
    #endregion

    #region IGrabbable
    public bool HandleCarriedState(Player currentPlayer, bool isCarried)
    {
        if (_isDying) return false;
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
            throwOnDestroy = null;
            actions.StopAnimation();
            actions.IsBaseActionActivated = false;
            if (!isInTuto)
            {
                StartCoroutine(TutoInGame());
            }
        }

        return true;
    }

    public void HandleDestroy()
    {
        if (_isDying) return;
        StartCoroutine(BreakPickaxe());
    }

    public GameObject GetGameObject() => gameObject;
    #endregion

    private IEnumerator BreakPickaxe()
    {
        _isDying = true;
        _GFX.SetActive(false);
        GameManager.Instance.NbPickaxe--;
        throwOnDestroy?.Invoke();
        Destroy(myTarget.gameObject);
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.2f, 0.2f);
        _pickaxePart.Play();
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
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
    private void PickaxeThrowSound()
    {
        RuntimeManager.PlayOneShot(pickaxeThrowSound, transform.position);
    }
    #endregion
}
