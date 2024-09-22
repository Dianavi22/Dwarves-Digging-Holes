using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField]
    private GameObject pickaxe;

    private Collider player = null;

    private GameObject createdPickaxe = null;

    private void OnTriggerEnter(Collider other)
    {
        GameObject collider = Utils.GetCollisionGameObject(other);

        if (collider.CompareTag("Player") && player == null)
        {
            player = other;
            Invoke(nameof(BuildPickaxe), 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collider = Utils.GetCollisionGameObject(other);

        if (collider.CompareTag("Player") && player != null)
        {
            player = null;
            CancelInvoke();
        }
    }

    public void BuildPickaxe() {
        if(createdPickaxe == null)
        {
            createdPickaxe = Instantiate(pickaxe, transform);
            player.gameObject.GetComponentInParent<PlayerActions>().TryPickUpObject();
        }
    }
}
