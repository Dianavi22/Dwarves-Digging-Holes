using System;
using UnityEngine;
using DG.Tweening;

public class Pickaxe : MonoBehaviour
{
    // In case the set of HealthPoint want to destroy the pickaxe
    // _healthPoint is update in GameManager
    private int _healthPoint = 1;
    public int HealthPoint
    {
        get => _healthPoint;
        set
        {
            _healthPoint = value;
            if (_healthPoint <= 0)
                Destroy(gameObject);
        }
    }

    public Action throwOnDestroy;

    public void Hit(GameObject hit)
    {
        if (Utils.TryGetParentComponent<Rock>(hit, out var rock))
        {
            HandleRockHit(rock);
        }
        
        else if (Utils.TryGetParentComponent<Player>(hit, out var player))
        {
            HandlePlayerHit(player);
        }
    }

    private void HandleRockHit(Rock rock)
    {
        rock.Hit();
        HealthPoint--;
        Debug.Log(HealthPoint);
    }

    private void HandlePlayerHit(Player player)
    {
        PlayerActions playerActions = player.GetActions();
        playerActions.ForceDetach();
        playerActions.Hit();
    }

    private void OnDestroy()
    {
        GameManager.Instance.NbPickaxe--;
        throwOnDestroy?.Invoke();
    }
}