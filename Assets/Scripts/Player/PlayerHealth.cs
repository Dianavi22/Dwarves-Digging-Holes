using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] public int currentHealth;

    private PlayerHealth allyToHeal;  
    private bool canHeal = false;

    void Start()
    {
        currentHealth = _maxHealth;
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && canHeal && allyToHeal != null)
        {
            HealAlly(allyToHeal);
        }
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
            if (allyToHeal != null && allyToHeal != this) 
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