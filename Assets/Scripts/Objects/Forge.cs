using System;
using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField] private GameObject pickaxePrefab;

    private Player _player;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (UIPauseManager.Instance.isPaused) return;
        if (_gameManager.PickaxeInstanceList.Count < _gameManager.MaxNbPickaxe && _player == null && Utils.TryGetParentComponent<Player>(other, out var player))
        {
            _player = player;
            Invoke(nameof(BuildPickaxe), 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_player != null && Utils.TryGetParentComponent<Player>(other, out _))
        {
            _player = null;
            CancelInvoke();
        }
    }

    public void BuildPickaxe()
    {
        var _createdPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        _gameManager.PickaxeInstanceList.Add(_createdPickaxe);
        _player.GetActions().TryPickUpObject();
    }
}
