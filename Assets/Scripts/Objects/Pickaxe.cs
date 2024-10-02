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
        
        else if (Utils.TryGetParentComponent<PlayerActions>(hit.transform.parent.gameObject, out var hitPlayerActions))
        {
            HandlePlayerHit(hitPlayerActions);
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

    private void HandlePlayerHit(PlayerActions playerActions)
    {
        playerActions.ForceDetach();
        playerActions.Hit();
    }

    private void DestroyPickaxe()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        throwOnDestroy?.Invoke();
        DOVirtual.DelayedCall(1f, () => respawnOnDestroy?.Invoke());
    }
}