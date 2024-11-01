using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using DG.Tweening;

[System.Serializable]
public class HealthChangedEvent : UnityEvent<int, int> { }

public class PlayerHealth : Player
{
    [SerializeField] GameObject _playerGFX;

    private bool _isReadyToSpawn = true;
    private RespawnPoint _respawnPoint;

    public bool IsAlive { private set; get; }

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

        movements.enabled = false;
        actions.enabled = false;
        actions.ForceDetach();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        EmptyPlayerFixedJoin();

        DOVirtual.DelayedCall(2f, () =>
        {
            _isReadyToSpawn = true;
        });
    }

    private void PlayerRespawn()
    {
        transform.SetPositionAndRotation(_respawnPoint.transform.position, Quaternion.identity);

        IsAlive = true;
        rb.useGravity = true;
        _playerGFX.SetActive(true);

        Invoke(nameof(Invincibility), 0.1f);
    }

    private void Invincibility()
    {
        movements.enabled = true;
        actions.enabled = true;
    }
}
