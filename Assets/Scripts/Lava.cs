using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] Collider _lavaCollider;
    private void Start()
    {
        _lavaCollider.enabled = false;
        Invoke("CooldownLava", 4);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerHealth>(other, out var playerHealth))
        {
            playerHealth.TakeDamage();
        }

        if (other.CompareTag("EndingCondition"))
        {
            GameManager.Instance.GameOver(1);
        }

        /*
         * Todo: Need to unify this condition
         * Why checking for all this tag when you can just destroy everything that enter in collision ? (exept some gameobject like player or chariot)
         */

        if (Utils.TryGetParentComponent<Rock>(other, out var rock))
        {
            Destroy(rock.gameObject);
        }

        if (Utils.TryGetParentComponent<Enemy>(other, out var enemy))
        {
            Destroy(enemy.gameObject);
        }

        if (Utils.TryGetParentComponent<Pickaxe>(other, out var pickaxe))
        {
            GameManager.Instance.PickaxeInstance = null;
            Destroy(pickaxe.gameObject);
        }
    }

    private void CooldownLava()
    {
        _lavaCollider.enabled = true;
    }
}
