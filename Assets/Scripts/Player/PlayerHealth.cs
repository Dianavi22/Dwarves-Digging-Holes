using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class HealthChangedEvent : UnityEvent<int, int> { }

public class PlayerHealth : MonoBehaviour
{
    public HealthChangedEvent onHealthChanged;
    private Transform _respawnPoint;
    private PlayerActions _playerActions;
    private PlayerMovements _playerMovements;
    private Rigidbody _rb;

    #region Old Heal system
    [SerializeField][HideInInspector] private int _maxHealth = 10;
    [SerializeField][HideInInspector] public int currentHealth;
    private PlayerHealth allyToHeal;
    private bool canHeal = false;
    private float healHoldTime = 0f;
    private float requiredHoldTime = 3f;
    #endregion

    public bool isAlive = true;
    private bool _isReadyToSpawn = true;
    [SerializeField] GameObject _playerGFX;

    void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        _playerMovements = GetComponent<PlayerMovements>();
        _rb = GetComponent<Rigidbody>();

        // Todo use TargetManager to find the GoldChariot
        _respawnPoint = TargetManager.Instance.GetGameObject(Target.RespawnPoint).transform;
        // _respawnPoint = FindObjectOfType<GoldChariot>().GetComponentInChildren<HitBoxRespawn>().gameObject.transform;
        currentHealth = _maxHealth;

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
        if (!isAlive && _isReadyToSpawn && _respawnPoint.GetComponent<HitBoxRespawn>().isReadyToRespawn)
        {
            PlayerRespawn();
        }
    }

    public void TakeDamage()
    {
        isAlive = false;
        StartCoroutine(DeathPlayer());
    }

    private IEnumerator DeathPlayer()
    {
        _isReadyToSpawn = false;
        _playerGFX.SetActive(false);

        _playerMovements.enabled = false;
        _playerActions.enabled = false;
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezePositionX;


        if (_playerActions.heldObject != null)
        {
            _playerActions.ForceDetach();
        }
        yield return new WaitForSeconds(2);

        _isReadyToSpawn = true;
    }


    private void PlayerRespawn()
    {

        this.transform.position = new Vector3(_respawnPoint.position.x, _respawnPoint.position.y, _respawnPoint.position.z);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        isAlive = true;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.constraints = RigidbodyConstraints.FreezePositionZ;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _playerGFX.SetActive(true);

        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(0.1f);
        _playerMovements.enabled = true;
        _playerActions.enabled = true;
    }
}
