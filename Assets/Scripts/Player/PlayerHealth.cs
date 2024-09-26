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
        this.GetComponent<PlayerMovements>().enabled = false;
        this.GetComponent<PlayerActions>().enabled = false;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        if (this.GetComponent<PlayerActions>().heldObject != gameObject.GetComponent<PlayerMovements>() && this.GetComponent<PlayerActions>().heldObject != null)
        {
            Destroy(this.GetComponent<PlayerActions>().heldObject.gameObject);
            this.GetComponent<PlayerActions>().heldObject = null;

        };
        if (this.GetComponent<PlayerActions>().heldObject == gameObject.GetComponent<PlayerMovements>())
        {
            this.GetComponent<PlayerActions>().heldObject.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            this.GetComponent<PlayerActions>().heldObject = null;
        };
        yield return new WaitForSeconds(2);

        _isReadyToSpawn = true;
    }


    private void PlayerRespawn()
    {
        this.transform.position = new Vector3(_respawnPoint.position.x, _respawnPoint.position.y, _respawnPoint.position.z);


        isAlive = true;
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        _playerGFX.SetActive(true);

        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<PlayerMovements>().enabled = true;
        this.GetComponent<PlayerActions>().enabled = true;
    }


    // & Feature added at the same time as the UI by Tristan for proper testing, we may not need it.

    private void InvokeOnHealthChanged()
    {
        onHealthChanged.Invoke(currentHealth, _maxHealth);
    }

    public void CheckPlayerLife()
    {
        if (currentHealth == 0)
        {
            Debug.Log("The player is dead.");
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        Debug.Log("Execution of player death actions.");
    }

    public void Damage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, _maxHealth);
        CheckPlayerLife(); 
        InvokeOnHealthChanged(); 
        Debug.Log("The player is Damage  = " + currentHealth);
    }

    public void Health(int health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, _maxHealth);
        InvokeOnHealthChanged(); 
        Debug.Log("The player is Health  = " + currentHealth);
    }
}
