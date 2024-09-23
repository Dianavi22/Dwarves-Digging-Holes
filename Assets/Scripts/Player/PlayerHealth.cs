using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Transform _respawnPoint;
    #region Old Heal system
    [SerializeField][HideInInspector] private int _maxHealth = 3;
    [SerializeField][HideInInspector] public int currentHealth;
    private PlayerHealth allyToHeal;
    private bool canHeal = false;
    private float healHoldTime = 0f;
    private float requiredHoldTime = 3f;
    #endregion

    public bool isAlive = true;
    [SerializeField] GameObject _playerGFX;


    void Start()
    {
        _respawnPoint = FindObjectOfType<HitBoxRespawn>().gameObject.transform;

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



    public void TakeDamage()
    {
        isAlive = false;
        StartCoroutine(DeathPlayer());
    }

    private IEnumerator DeathPlayer()
    {

        _playerGFX.SetActive(false);
        this.GetComponent<PlayerMovements>().enabled = false;
        this.GetComponent<PlayerActions>().enabled = false;
        this.GetComponent<Rigidbody>().useGravity = false;
        if (this.GetComponent<PlayerActions>().heldObject != gameObject.GetComponent<PlayerMovements>())
        {
            Destroy(this.GetComponent<PlayerActions>().heldObject.gameObject);
            this.GetComponent<PlayerActions>().heldObject = null;

        };
        if (this.GetComponent<PlayerActions>().heldObject == gameObject.GetComponent<PlayerMovements>())
        {
            this.GetComponent<PlayerActions>().heldObject.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            this.GetComponent<PlayerActions>().heldObject = null;
        };
        yield return new WaitForSeconds(5);
        PlayerRespawn();
    }

    private void PlayerRespawn()
    {
        this.transform.position = new Vector3(_respawnPoint.position.x, _respawnPoint.position.y, _respawnPoint.position.z);
        _playerGFX.SetActive(true);


        isAlive = true;
        this.GetComponent<Rigidbody>().useGravity = true;

        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(3);
        this.GetComponent<PlayerMovements>().enabled = true;
        this.GetComponent<PlayerActions>().enabled = true;
    }
}
