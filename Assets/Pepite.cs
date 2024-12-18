using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepite : MonoBehaviour
{
    private bool _isDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player) && !_isDestroy)
        {
            _isDestroy = true;
            TargetManager.Instance.GetGameObject<GoldChariot>().GoldCount++;
            Destroy(gameObject);
        }

        if (Utils.Component.TryGetInParent<Enemy>(collision.collider, out var enemy) && !_isDestroy)
        {
            _isDestroy = true;
            Destroy(gameObject);
        }

        if (Utils.Component.TryGetInParent<Lava>(collision.collider, out var lava) && !_isDestroy)
        {
            _isDestroy = true;
            Destroy(gameObject);
        }
    }
}
