using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] public int currentHealth;

    private PlayerHealth allyToHeal;
    private bool canHeal = false;

    private float healHoldTime = 0f;
    private float requiredHoldTime = 3f;

    void Start()
    {
        currentHealth = _maxHealth;
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
}
