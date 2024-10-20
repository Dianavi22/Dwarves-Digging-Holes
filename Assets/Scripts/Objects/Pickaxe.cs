using System;
using UnityEngine;
using DG.Tweening;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private int _healthPoint = 20;

    public Action throwOnDestroy;
    public Action respawnOnDestroy;

    public void Hit(GameObject hit)
    {
        if (Utils.TryGetParentComponent<Rock>(hit, out var rock))
        {
            HandleRockHit(rock);
        }
        
        else if (Utils.TryGetParentComponent<Player>(hit.transform.parent.gameObject, out var player))
        {
            HandlePlayerHit(player);
        }
    }

    private void HandleRockHit(Rock rock)
    {
        rock.Hit();
        _healthPoint -= 1;

        if (_healthPoint <= 0)
        {
            DestroyPickaxe();
        }

        Debug.Log(_healthPoint);
    }

    private void HandlePlayerHit(Player player)
    {
        PlayerActions playerActions = player.GetActions();
        playerActions.ForceDetach();
        playerActions.Hit();
    }

    private void DestroyPickaxe()
    {
        GameManager.Instance.PickaxeInstanceList.Remove(gameObject);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        throwOnDestroy?.Invoke();
        DOVirtual.DelayedCall(1f, () => respawnOnDestroy?.Invoke());
    }
}