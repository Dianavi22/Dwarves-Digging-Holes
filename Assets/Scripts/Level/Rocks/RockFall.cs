using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] ParticleSystem _breakPart;
    private bool _isDestroyed = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!_isDestroyed)
        {
            if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player))
            {
                player.HandleDestroy();
                StartCoroutine(DestroyRock());

            }
            if (Utils.Component.TryGetInParent<GoldChariot>(collision.collider, out var goldChariot) || Utils.Component.TryGetInParent<MoreGold>(collision.collider, out var mg))
            {
                _isDestroyed = true;
                StartCoroutine(DestroyRock());
                goldChariot.LostGoldByRock();
            }

            if (Utils.Component.TryGetInParent<Platform>(collision.collider, out var platform))
            {
                StartCoroutine(DestroyRock());

            }
            if (Utils.Component.TryGetInParent<Rock>(collision.collider, out var rock))
            {
                StartCoroutine(DestroyRock());

            }
            if (Utils.Component.TryGetInParent<Enemy>(collision.collider, out var enemy))
            {
                enemy.HandleDestroy();
                StartCoroutine(DestroyRock());
            }
        }
    }

    private IEnumerator DestroyRock()
    {
        _isDestroyed = true;
        _gfx.SetActive(false);
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _breakPart.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

}