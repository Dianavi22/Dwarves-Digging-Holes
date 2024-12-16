using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepite : MonoBehaviour
{
    private GoldChariot _gc;
    private bool _isDestroy;
    private void Start()
    {
        _gc = FindAnyObjectByType<GoldChariot>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player) && !_isDestroy)
        {
            _isDestroy = true;
            _gc.AddGoldPepite();
            Destroy(gameObject);
        }
    }
}
