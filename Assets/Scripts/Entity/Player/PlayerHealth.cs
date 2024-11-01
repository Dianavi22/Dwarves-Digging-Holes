using UnityEngine;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject _playerGFX;

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
        _respawnPoint = TargetManager.Instance.GetGameObject(Target.RespawnPoint).GetComponent<RespawnPoint>();

        IsAlive = true;
    }

    private void Update()
    {
        if (!IsAlive && _isReadyToSpawn && _respawnPoint.IsReadyToRespawn)
        {
            PlayerRespawn();
        }
    }

    public void TakeDamage()
    {
        DeathPlayer();
    }

    private void DeathPlayer()
    {
        IsAlive = false;
        _isReadyToSpawn = false;
        _playerGFX.SetActive(false);

        _p.GetMovement().enabled = false;
        _p.GetActions().enabled = false;
        _p.GetActions().ForceDetach();
        _p.GetRigidbody().useGravity = false;
        _p.GetRigidbody().velocity = Vector3.zero;

        _p.EmptyPlayerFixedJoin();

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
