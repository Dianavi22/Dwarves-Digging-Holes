using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] public int currentHealth;
    [SerializeField] private bool _isNearDwarf;

    [SerializeField] private Dwarf _dwarf;

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
            print("HEAL");

            if (_isNearDwarf)
            {

                GiveHealth(); // Soigne si près du nain
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H) && _isNearDwarf)
        {
            GiveHealth(); // Soigner avec H si près du nain
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            TakeDamage(); // Subir des dégâts avec J
        }
    }

    public void TakeDamage()
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
        _healed = true;
        yield return new WaitForSeconds(2); // 2 secondes d'invincibilité
        _healed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Dwarf>() != null)
        {
            _dwarf = collision.collider.GetComponent<Dwarf>();
            print("HEAL");
            // Vérifie si c'est un nain grâce au composant Dwarf

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

            print("DAMAGE");

            TakeDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isNearDwarf = false;
    }
}
