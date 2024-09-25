using System;
using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField]
    private GameObject pickaxePrefab;

    private PlayerActions player;
    private GameObject createdPickaxe;

    private void Start()
    {
        createdPickaxe = GameManager.Instance.PickaxeInstance;
    }

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
        createdPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        GameManager.Instance.PickaxeInstance = createdPickaxe;
        player.TryPickUpObject();
    }
}
