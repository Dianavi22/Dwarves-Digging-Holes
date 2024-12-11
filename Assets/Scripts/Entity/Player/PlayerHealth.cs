using UnityEngine;
using DG.Tweening;
using Utils;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject _playerGFX;
    [SerializeField] private ParticleSystem _HurtPart;
    [SerializeField] private ParticleSystem _DestroyPlayer;

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
        _respawnPoint = TargetManager.Instance.GetGameObject<RespawnPoint>(Target.RespawnPoint);

        IsAlive = true;
    }

    private void Update()
    {
        if (!IsAlive && _isReadyToSpawn && _respawnPoint.IsReadyToRespawn)
        {
            TriggerRespawnSequence();
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
        
        DOVirtual.DelayedCall(1f, () =>
        {
            _p.GetMovement().enabled = true;
            _isHit = false;
        });
    }

    public void DeathPlayer()
    {
    
        IsAlive = false;
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.2f,0.2f);
        _DestroyPlayer.Play();
        _isReadyToSpawn = false;
        _playerGFX.SetActive(false);

        _p.GetMovement().enabled = false;
        _p.GetActions().enabled = false;
        _p.GetActions().ForceDetach();
        _p.GetRigidbody().useGravity = false;
        _p.GetRigidbody().velocity = Vector3.zero;

        _p.EmptyPlayerFixedJoin();

        StatsManager.Instance.IncrementStatistic(_p, StatsName.MostDeath, 1);
        if(_p.holdBy != null) {
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
        _playerGFX.SetActive(true);

        Invoke(nameof(Invincibility), 0.1f);
    }

    private void Invincibility()
    {
        _p.GetMovement().enabled = true;
        _p.GetActions().enabled = true;
    }
}
