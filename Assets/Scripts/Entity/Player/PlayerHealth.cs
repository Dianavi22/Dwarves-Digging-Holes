using UnityEngine;
using DG.Tweening;
using Utils;
using FMODUnity;
using FMOD.Studio; 


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private ParticleSystem _HurtPart;
    [SerializeField] private ParticleSystem _DestroyPlayer;
    [SerializeField] private RotateImage _imageRespawn;
    [SerializeField] private EventReference resurgenceSound;
    [SerializeField] private EventReference deathSound;


    private bool _isHit = false;
    private bool _isReadyToSpawn = true;
    private RespawnPoint _respawnPoint;
    private Player _p;

    public bool IsAlive { private set; get; }

    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    void Start()
    {
        IsAlive = true;
        if (GameManager.Instance.isInMainMenu)
        { 
            this.enabled = false;
            return;
        }
        _respawnPoint = TargetManager.Instance.GetGameObject<RespawnPoint>();
        _imageRespawn = FindAnyObjectByType<RotateImage>();
    }

    private void Update()
    {

        if (this.transform.rotation.y == -180)
        {
            _HurtPart.transform.rotation = Quaternion.Euler(-90, 0, 180);
        }
        else
        {
            _HurtPart.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        if (!IsAlive && _isReadyToSpawn && _respawnPoint.IsReadyToRespawn)
        {
            TriggerRespawnSequence();
        }
        else
        {
            _imageRespawn.isRespawn = false;
        }

        if (!IsAlive)
        {
            _imageRespawn.isRespawn = true;
        }
        else
        {
            _imageRespawn.isRespawn = false;
        }

        if (!IsAlive && _isReadyToSpawn)
        {
            _imageRespawn.isImpossibleRespawn = true;
        }
        else
        {
            _imageRespawn.isImpossibleRespawn = false;
        }
    }

    private void TriggerRespawnSequence()
    {
        _respawnPoint.circle.transform.DOKill();

        Sequence respawnSequence = DOTween.Sequence();
        respawnSequence.AppendCallback(PlayerRespawn)
            .Append(_respawnPoint.circle.transform.DOScale(2f, 0.33f).SetEase(Ease.OutQuad))
            .Append(_respawnPoint.circle.transform.DOScale(0f, 0.33f).SetEase(Ease.InQuad));
    }
    public void Stun()
    {
        if (_isHit) return;

        _HurtPart.Play();
        _isHit = true;
        _p.GetMovement().enabled = false;
        _p.GetAnimator().SetTrigger("stunned");
       // StartCoroutine(this.GetComponent<PlayerActions>().Taunt());
        DOVirtual.DelayedCall(1f, () =>
        {
            _p.GetMovement().enabled = true;
            _isHit = false;
            _HurtPart.Stop();
        });
    }

    public void DeathPlayer()
    {
        DeathSound();

        IsAlive = false;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(0.2f, 0.2f);
        _DestroyPlayer.Play();
        _isReadyToSpawn = false;
        _p.GetModelRef().gameObject.SetActive(false);

        _p.GetMovement().enabled = false;
        _p.GetActions().ForceDetach();
        _p.GetActions().enabled = false;
        _p.GetRigidbody().useGravity = false;
        _p.GetRigidbody().velocity = Vector3.zero;

        _p.EmptyFixedJoin();

        StatsManager.Instance.IncrementStatistic(_p, StatsName.MostDeath, 1);
        if (_p.holdBy != null)
        {
            StatsManager.Instance.IncrementStatistic(_p.holdBy, StatsName.PlayerKill, 1);
            _p.holdBy = null;
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            _isReadyToSpawn = true;
        });
    }

    private void PlayerRespawn()
    {
        transform.SetPositionAndRotation(_respawnPoint.transform.position, Quaternion.identity);
        IsAlive = true;
        _p.GetRigidbody().useGravity = true;
        _p.GetModelRef().gameObject.SetActive(true);
        Invoke(nameof(Invincibility), 0.1f);

        ResurgenceSound();
    }

    private void Invincibility()
    {
        _p.GetMovement().enabled = true;
        _p.GetActions().enabled = true;
    }


    #region Sounds
    private void ResurgenceSound()
    {
        RuntimeManager.PlayOneShot(resurgenceSound, transform.position);
    }

    private void DeathSound()
    {
        RuntimeManager.PlayOneShot(deathSound, transform.position);
    }
    #endregion
}
