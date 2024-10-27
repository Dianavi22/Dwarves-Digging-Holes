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
    public HealthChangedEvent onHealthChanged; // Not used for now
    private RespawnPoint _respawnPoint;

    #region Old Heal system
    [SerializeField][HideInInspector] private int _maxHealth = 10;
    [SerializeField][HideInInspector] public int currentHealth;
    private PlayerHealth allyToHeal;
    private bool canHeal = false;
    private float healHoldTime = 0f;
    private float requiredHoldTime = 3f;
    #endregion

    public bool IsAlive { private set; get; }
    private bool _isReadyToSpawn = true;
    [SerializeField] GameObject _playerGFX;

    void Start()
    {
        _respawnPoint = TargetManager.Instance.GetGameObject(Target.RespawnPoint).GetComponent<RespawnPoint>();
        currentHealth = _maxHealth;

        IsAlive = true;

        onHealthChanged.Invoke(currentHealth, _maxHealth);
    }

    #region Old heal system
    public void OnHeal(InputAction.CallbackContext context)
    {
        if (canHeal && allyToHeal != null)
        {
            if (context.phase == InputActionPhase.Started)
            {
                healHoldTime = 0f;
            }

            if (context.phase == InputActionPhase.Performed)
            {
                StartCoroutine(HealingWaiting());
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                healHoldTime = 0f;
            }
        }
    }

    private IEnumerator HealingWaiting()
    {
        yield return new WaitForSeconds(requiredHoldTime);
        HealAlly(allyToHeal);
    }

    private void HealAlly(PlayerHealth ally)
    {
        ally.currentHealth = Mathf.Min(ally.currentHealth + 1, ally._maxHealth);
    }



    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        allyToHeal = collision.collider.GetComponent<PlayerHealth>();
    //        if (allyToHeal != null && allyToHeal != this && allyToHeal.currentHealth != _maxHealth)
    //        {
    //            canHeal = true;
    //        }
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        canHeal = false;
    //        allyToHeal = null;
    //    }
    //}
    #endregion

    private void Update()
    {
        if (!IsAlive && _isReadyToSpawn && _respawnPoint.isReadyToRespawn)
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
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        if (actions.heldObject != null)
        {
            actions.ForceDetach();
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
