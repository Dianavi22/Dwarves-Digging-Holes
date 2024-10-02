using System;
using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField]
    private GameObject pickaxePrefab;

    private PlayerActions _player;
    private GameObject _createdPickaxe;

    private void Start()
    {
        _createdPickaxe = GameManager.Instance.PickaxeInstance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_createdPickaxe == null && _player == null && Utils.TryGetParentComponent<PlayerActions>(other, out var playerActions))
        {
            _player = playerActions;
            Invoke(nameof(BuildPickaxe), 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_player != null && Utils.TryGetParentComponent<PlayerActions>(other, out _))
        {
            _player = null;
            CancelInvoke();
        }
    }

    public void BuildPickaxe()
    {
        _createdPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        GameManager.Instance.PickaxeInstance = _createdPickaxe;
        _player.TryPickUpObject();
    }
}
