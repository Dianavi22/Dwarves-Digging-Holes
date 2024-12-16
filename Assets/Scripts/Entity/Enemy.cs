using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;
using FMOD.Studio;
using FMODUnity;

public class Enemy : Entity
{
    [SerializeField] private EventReference goblinLaughSound;
    [SerializeField] private EventReference goblinDeadSound;
    [SerializeField] private EventReference goblinPeriodicSound;
    private Coroutine periodicSoundCoroutine;

    [SerializeField] ParticleSystem _destroyGobPart;
    [SerializeField] GameObject _gfx;

    [HideInInspector] public GoldChariot _goldChariot;
    private bool _isTouchChariot;
    [HideInInspector] public bool canSteal = true;
    [SerializeField] Tuto _tuto;
    [SerializeField] GameManager _gameManager;
    [SerializeField] Lava _lava;
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
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
        _tuto = FindAnyObjectByType<Tuto>();
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(PlayGoblinLaughWithDelay());
        periodicSoundCoroutine = StartCoroutine(PeriodicSoundLoop());

    }


    private IEnumerator PlayGoblinLaughWithDelay()
    {
        float randomDelay = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomDelay);

        EventInstance laughInstance = RuntimeManager.CreateInstance(goblinLaughSound);
        RuntimeManager.AttachInstanceToGameObject(laughInstance, transform, GetComponent<Rigidbody>());
        laughInstance.start();
        laughInstance.release();
    }

    private IEnumerator PeriodicSoundLoop()
    {
        while (!_isDead)
        {
            float randomDelay = Random.Range(5f, 10f);
            yield return new WaitForSeconds(randomDelay);

            if (!_isDead)
            {
                EventInstance periodicInstance = RuntimeManager.CreateInstance(goblinPeriodicSound);
                RuntimeManager.AttachInstanceToGameObject(periodicInstance, transform, GetComponent<Rigidbody>());
                periodicInstance.start();
                periodicInstance.release();
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
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.3f, 0.3f);
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

    private void DeadSound()
    {  
        EventInstance deadInstance = RuntimeManager.CreateInstance(goblinDeadSound);
        RuntimeManager.AttachInstanceToGameObject(deadInstance, transform, GetComponent<Rigidbody>());
        deadInstance.start();
        deadInstance.release();
        Debug.Log("DeadSound");
    }
}
