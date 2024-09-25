using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField]
    private GameObject pickaxe;

    private PlayerActions player = null;
    private GameObject createdPickaxe = null;

    private void OnTriggerEnter(Collider other)
    {
        if (createdPickaxe == null && player == null && Utils.TryGetParentComponent<PlayerActions>(other, out var _object))
        {
            player = _object;
            Invoke(nameof(BuildPickaxe), 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && Utils.TryGetParentComponent<PlayerActions>(other, out var _object))
        {
            player = null;
            CancelInvoke();
        }
    }

    public void BuildPickaxe() {
        createdPickaxe = Instantiate(pickaxe, transform);
        player.TryPickUpObject();
    }
}
