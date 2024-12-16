using System;
using UnityEngine;

public class Forge : MonoBehaviour
{
    [SerializeField] private Pickaxe pickaxePrefab;
    private GameManager _gameManager;
    [SerializeField] private GameObject _bubblePickaxe;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (_gameManager.CanCreatePickaxe)
        {
            _bubblePickaxe.SetActive(true);
        }
        else
        {
            _bubblePickaxe.SetActive(false);

        }
    }

    public void BuildPickaxe(PlayerActions _player)
    {
        var _createdPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        _gameManager.AddPickaxe(_createdPickaxe);
        _player.TryPickUpObject();
    }
}
