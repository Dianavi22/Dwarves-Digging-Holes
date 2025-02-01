using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] ParticleSystem _breakPart;
    private bool _isDestroyed = false;

    public Vector3 startRotation;
    public float duration = 2f;

    private float timeElapsed = 0f;
    private bool rotating = true;
    private int endRotation;

    private void Start()
    {
        endRotation = Random.Range(-500, 500);


    }
    private void Update()
    {
        if (rotating)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            _gfx.transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(new Vector3(0,0, endRotation)), t);

            if (t >= 1.0f)
            {
                rotating = false;
            }
        }
    }
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
                goldChariot.DamageByFallRock();
                StartCoroutine(DestroyRock());
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
