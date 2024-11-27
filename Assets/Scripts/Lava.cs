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
        if (Utils.TryGetParentComponent<IGrabbable>(other, out var grabbable))
        {
            grabbable.HandleDestroy();
        }

        if (Utils.TryGetParentComponent<Rock>(other, out var rock))
        {
            Destroy(rock.gameObject);

        }
        
        if (other.CompareTag("EndingCondition"))
        {
            GameManager.Instance.GameOver(DeathMessage.Lava);
        }
    }

    private void CooldownLava()
    {
        _lavaCollider.enabled = true;
    }
}
