using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RockFall : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] ParticleSystem _breakPart;
    [SerializeField] ParticleSystem _spawnRockFall;

    [SerializeField] private EventReference RockAppearance;
    [SerializeField] private EventReference RocksFall;

    private bool _isDestroyed = false;

    public Vector3 startRotation;
    public float duration = 2f;

    private float timeElapsed = 0f;
    private bool rotating = true;
    private int endRotation;

    private float _speedModifier = 1;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        endRotation = Random.Range(-500, 500);
        _spawnRockFall.Play();

        RockAppearanceSound();
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
    private void FixedUpdate()
    {

        Physics.SyncTransforms();
        Vector3 goalDestination = GameManager.Instance.CurrentScrollingSpeed * _speedModifier * Time.fixedDeltaTime * Vector3.left;
        _rb.MovePosition(transform.position + goalDestination);
        if (GameManager.Instance.isGameOver)
        {
            _speedModifier = 0;
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
        RocksFallSound();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }


    #region Sounds
    private void RockAppearanceSound()
    {
        EventInstance RockAppearanceInstance = RuntimeManager.CreateInstance(RockAppearance);
        RuntimeManager.AttachInstanceToGameObject(RockAppearanceInstance, transform, GetComponent<Rigidbody>());
        RockAppearanceInstance.start();
        RockAppearanceInstance.release();
    }

    private void RocksFallSound()
    {
        EventInstance RocksFallInstance = RuntimeManager.CreateInstance(RocksFall);
        RuntimeManager.AttachInstanceToGameObject(RocksFallInstance, transform, GetComponent<Rigidbody>());
        RocksFallInstance.start();
        RocksFallInstance.release();
    }

    #endregion

}
