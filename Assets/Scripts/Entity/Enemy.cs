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

    [SerializeField] List<Collider> _colliders;

    private Tuto _tuto;

    private GoldChariot _goldChariot;
    public Vector3 GetDestinationPosition => _goldChariot.transform.position;
    private bool _isTouchChariot;
    [HideInInspector] public bool canSteal = true;

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

    private void Start()
    {
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _tuto = TargetManager.Instance.GetGameObject<Tuto>();
        StartCoroutine(PlayGoblinLaughWithDelay());
        StartCoroutine(PeriodicSoundLoop());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<GoldChariot>(collision.gameObject, out _))
        {
            Debug.Log("Touching chariot");
            IsTouchingChariot = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (Utils.Component.TryGetInParent<GoldChariot>(collision.gameObject, out _) && IsTouchingChariot)
        {
            Debug.Log("---Leaving chariot");
            IsTouchingChariot = false;
        }
    }

    public IEnumerator HitChariot()
    {
        if (_isDead) yield break;

        _goldChariot.GoldCount -= 1;
        canSteal = false;
        _goldChariot.oneLostPart.Play();
            
        StealingSound();
        LaughSound();

        yield return new WaitForSeconds(1);
        canSteal = true;
    }

    public IEnumerator DestroyByLava()
    {
        _isDead = true;
        for (int i = 0; i < _colliders.Count; i++)
        {
            _colliders[i].enabled = false;
        }

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

        if (_tuto.isTakeEnemy) _tuto.isYeetEnemy = true;

        if (grabbed) 
        {
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
            GameManager.Instance.SkipTuto();
        }
        
        StartCoroutine(DestroyByLava());
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
    private IEnumerator PlayGoblinLaughWithDelay()
    {
        float randomDelay = Random.Range(0f, 2f);
        yield return new WaitForSeconds(randomDelay);

        LaughSound();
    }

    #region Sounds
    private void LaughSound()
    {  
        EventInstance laughInstance = RuntimeManager.CreateInstance(goblinLaughSound);
        RuntimeManager.AttachInstanceToGameObject(laughInstance, transform, GetRigidbody());
        laughInstance.start();
        laughInstance.release();
    }

    private void PeriodicSound()
    {  
        EventInstance periodicInstance = RuntimeManager.CreateInstance(goblinPeriodicSound);
        RuntimeManager.AttachInstanceToGameObject(periodicInstance, transform, GetRigidbody());
        periodicInstance.start();
        periodicInstance.release();
    }

    private void StealingSound()
    {  
        EventInstance stealingInstance = RuntimeManager.CreateInstance(goblinStealingSound);
        RuntimeManager.AttachInstanceToGameObject(stealingInstance, transform, GetRigidbody());
        stealingInstance.start();
        stealingInstance.release();
    }

    private void DeadSound()
    {  
        EventInstance deadInstance = RuntimeManager.CreateInstance(goblinDeadSound);
        RuntimeManager.AttachInstanceToGameObject(deadInstance, transform, GetRigidbody());
        deadInstance.start();
        deadInstance.release();
    }

    #endregion
}


