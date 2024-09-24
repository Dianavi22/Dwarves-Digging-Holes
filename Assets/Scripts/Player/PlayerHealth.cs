using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class HealthChangedEvent : UnityEvent<int, int> { }

public class PlayerHealth : MonoBehaviour
{
    public HealthChangedEvent onHealthChanged;

    [SerializeField] public int _maxHealth = 30;
    [SerializeField] public int currentHealth;

    private PlayerHealth allyToHeal;
    private bool canHeal = false;

    private float healHoldTime = 0f;
    private float requiredHoldTime = 3f;

    public PlayerInformationManager playerInformationManager;

    void Start()
    {
        currentHealth = _maxHealth;

        onHealthChanged.Invoke(currentHealth, _maxHealth);
    }

    void Update()
    {

    }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            allyToHeal = collision.collider.GetComponent<PlayerHealth>();
            if (allyToHeal != null && allyToHeal != this && allyToHeal.currentHealth != _maxHealth)
            {
                canHeal = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            canHeal = false;
            allyToHeal = null;
        }
    }

    public void TakeDamage()
    {
        currentHealth = Mathf.Max(currentHealth - 1, 0);
    }


    // & Feature added at the same time as the UI by Tristan for proper testing, we may not need it.

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
        onHealthChanged.Invoke(currentHealth, _maxHealth); 
        Debug.Log("The player is Damage  = " + currentHealth);
    }

    public void Health(int health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, _maxHealth);
        onHealthChanged.Invoke(currentHealth, _maxHealth); 
        Debug.Log("The player is Health  = " + currentHealth);
    }























}
