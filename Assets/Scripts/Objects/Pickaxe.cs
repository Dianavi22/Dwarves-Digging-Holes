using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;


public class Pickaxe : MonoBehaviour
{
    [SerializeField]
    private int _healthPoint = 20;

    public Action throwOnDestroy;

    public Action respawnOnDestroy;

    public void Hit(GameObject hit)
    {
        if (Utils.TryGetParentComponent<Rock>(hit, out var rock))
        {
            rock.Hit();
            _healthPoint -= 1;
            if (_healthPoint <= 0)
            {
                Break();
            }
            Debug.Log(_healthPoint);
        }
        else if(Utils.TryGetParentComponent<PlayerActions>(hit.transform.parent.gameObject, out var hitPlayerActions))
        {
            hitPlayerActions.ForceDetach();
            hitPlayerActions.Hit();
        }
    }
     
    public void Break()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        throwOnDestroy?.Invoke();
        DOVirtual.DelayedCall(1f, () => { respawnOnDestroy?.Invoke(); });
    }

}