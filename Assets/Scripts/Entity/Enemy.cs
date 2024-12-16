using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils;
using FMOD.Studio;
using FMODUnity;

public class Enemy : Entity
{
    [Header("Sound effect")]
    [SerializeField] private EventReference goblinLaughSound;
    [SerializeField] private EventReference goblinStealingSound;
    [SerializeField] private EventReference goblinDeadSound;
    [SerializeField] private EventReference goblinPeriodicSound;

    [Header("Particle effect")]
    [SerializeField] ParticleSystem _destroyGobPart;
    [SerializeField] GameObject _gfx;

    [SerializeField] Tuto _tuto;

    [HideInInspector] public GoldChariot _goldChariot;
    private bool _isTouchChariot;
    [HideInInspector] public bool canSteal = true;

    private GameManager _gameManager;
    private bool _isDead = false;
    public bool IsTouchingChariot
    {
        get => _isTouchChariot;
        set
        {
            if (_isTouchChariot == value) return;
            if (value) _goldChariot.NbGoblin++;
            else _goldChariot.NbGoblin--;
            _isTouchChariot = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _tuto = FindAnyObjectByType<Tuto>();
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(PlayGoblinLaughWithDelay());
        StartCoroutine(PeriodicSoundLoop());
    }

    private IEnumerator PeriodicSoundLoop()
    {
        while (!_isDead)
        {
            float randomDelay = Random.Range(3f, 7f);
            yield return new WaitForSeconds(randomDelay);

            if (!_isDead)
            {
                PeriodicSound();
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            if(!IsGrabbed){_rb.isKinematic = true;};
            IsTouchingChariot = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            IsTouchingChariot = false;
            if(!IsGrabbed){_rb.isKinematic = false; };
        }
    }

    public IEnumerator HitChariot()
    {
        if (!_isDead)
        {
            _goldChariot.GoldCount -= 1;
            canSteal = false;
            _goldChariot.oneLostPart.Play();
            
            StealingSound();
            LaughSound();

            yield return new WaitForSeconds(1);
            canSteal = true;
        }
    }

    public void KillGobs()
    {
        _gfx.SetActive(false);
        _rb.velocity = Vector3.zero;
    }

    public IEnumerator DestroyByLava()
    {
        _isDead = true;
        this.GetComponentInChildren<Collider>().enabled = false;

        if (holdBy != null)
        {
            StatsManager.Instance.IncrementStatistic(holdBy, StatsName.GoblinKill, 1);
            holdBy = null;
        };

        _rb.velocity = Vector3.zero;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.3f, 0.3f);
        _rb.isKinematic = true;
        _gfx.SetActive(false);
        _destroyGobPart.Play();

        DeadSound();
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }

    public override void HandleCarriedState(Player player, bool grabbed) {

        if (_tuto.isTakeEnemy)
        {
            _tuto.isYeetEnemy = true;
        }
        if (grabbed) {
            IsTouchingChariot = false;
        }
        base.HandleCarriedState(player, grabbed);
        
        _rb.mass = grabbed ? 1f : 5f;
    } 
    override public void HandleDestroy()
    {
        if (_isDead) return;

        if (_tuto.isYeetEnemy)
        {
            _tuto.isYeetEnemy = false;
            _tuto.StopTuto();
        }
        if(_tuto.isTakeEnemy)
        {
            _tuto.StopTuto();
            _gameManager.SkipTuto();
        }
        
        StartCoroutine(DestroyByLava());
    }


    private IEnumerator PlayGoblinLaughWithDelay()
    {
        float randomDelay = Random.Range(0f, 2f);
        yield return new WaitForSeconds(randomDelay);

        LaughSound();
    }

    private void LaughSound()
    {  
        EventInstance laughInstance = RuntimeManager.CreateInstance(goblinLaughSound);
        RuntimeManager.AttachInstanceToGameObject(laughInstance, transform, GetComponent<Rigidbody>());
        laughInstance.start();
        laughInstance.release();
    }

    private void PeriodicSound()
    {  
        EventInstance periodicInstance = RuntimeManager.CreateInstance(goblinPeriodicSound);
        RuntimeManager.AttachInstanceToGameObject(periodicInstance, transform, GetComponent<Rigidbody>());
        periodicInstance.start();
        periodicInstance.release();
    }

    private void StealingSound()
    {  
        EventInstance stealingInstance = RuntimeManager.CreateInstance(goblinStealingSound);
        RuntimeManager.AttachInstanceToGameObject(stealingInstance, transform, GetComponent<Rigidbody>());
        stealingInstance.start();
        stealingInstance.release();
    }

    private void DeadSound()
    {  
        EventInstance deadInstance = RuntimeManager.CreateInstance(goblinDeadSound);
        RuntimeManager.AttachInstanceToGameObject(deadInstance, transform, GetComponent<Rigidbody>());
        deadInstance.start();
        deadInstance.release();
    }
}
