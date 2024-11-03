using System;
using UnityEngine;
using static UnityEngine.Rendering.ReloadAttribute;

public class Pickaxe : MonoBehaviour, IGrabbable
{
    [SerializeField] private int _healthPoint = 20;

    private Action throwOnDestroy;

    public void HandleCarriedState(Player currentPlayer, bool isCarried)
    {
        PlayerActions actions = currentPlayer.GetActions();
        if (isCarried)
        {
            throwOnDestroy = () => { actions.EmptyHands(); actions.StopAnimation(); actions.IsBaseActionActivated = false; };
        }
        else
        {
            actions.StopAnimation();
            actions.IsBaseActionActivated = false;
        }
    }

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
    }
}