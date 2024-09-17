using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] public int currentHealth;
    [SerializeField] private bool _isNearDwarf;

    private int _hpDwarf;
    private bool _healed = false;
    void Start()
    {
        currentHealth = _maxHealth;
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            print("TOUCHE");

            if (_isNearDwarf)
            {
            }
            else
            {
            }
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            print("TOUCHE");

           _isNearDwarf = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H) && _isNearDwarf)
        {
            TakeDamage();
        }
        if (Input.GetKeyUp(KeyCode.J) )
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        currentHealth--;
    }

    private void GiveHealth()
    {
        _hpDwarf++;
        _healed = true;
    }

    private IEnumerator Invincibility()
    {
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _hpDwarf = collision.collider.GetComponent<PlayerHealth>().currentHealth;
            _isNearDwarf = true;

            if (_healed)
            {
                collision.collider.GetComponent<PlayerHealth>().currentHealth = _hpDwarf;
                _healed = false;
            }
        }
        if (collision.collider.CompareTag("Lava"))
        {
            TakeDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isNearDwarf = false;
    }

}
