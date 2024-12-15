using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    private bool _isDestroyed = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player) && !_isDestroyed)
        {
            player.HandleDestroy();
            StartCoroutine(DestroyRock());

        }
        if (Utils.Component.TryGetInParent<GoldChariot>(collision.collider, out var goldChariot) && !_isDestroyed)
        {
            print("goldChariot");
            StartCoroutine(DestroyRock());

        }
        if (Utils.Component.TryGetInParent<Platform>(collision.collider, out var platform) && !_isDestroyed)
        {
            StartCoroutine(DestroyRock());

        }
        if (Utils.Component.TryGetInParent<Rock>(collision.collider, out var rock) && !_isDestroyed)
        {
            StartCoroutine(DestroyRock());
        }

    }

    private IEnumerator DestroyRock()
    {
        _isDestroyed = true;
        _gfx.SetActive(false);
        this.GetComponent<Collider>().isTrigger = true;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
