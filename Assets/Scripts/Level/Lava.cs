using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private Collider _lavaCollider;
    [SerializeField] private ShakyCame _sc;
    [SerializeField] private EventReference lavaSound;
    [SerializeField] private EventReference lavaBurntSound;
    [SerializeField] ParticleSystem _rockFall;
    private EventInstance _lavaEventInstance;
    private bool _isStartLava;

    private void Start()
    {
        _lavaCollider.enabled = false;
        StartCoroutine(CooldownLava());
        PlayLavaSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<IGrabbable>(other, out var grabbable))
        {
            PlayLavaBurntSound();
            grabbable.HandleDestroy();
        }

        if (other.CompareTag("EndingCondition"))
        {
            StartCoroutine(GameManager.Instance.GameOver(DeathMessage.Lava));
        }

        /*
         * Todo: Need to unify this condition
         * Why checking for all this tag when you can just destroy everything that enter in collision ? (exept some gameobject like player or chariot)
         */

        if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
        {
            Destroy(rock.gameObject);
        }
    }
    private void Update()
    {
        if (_isStartLava)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Time.deltaTime * 0.5f);
        }
    }

    private IEnumerator CooldownLava()
    {
        yield return new WaitForSeconds(4);
        _rockFall.Play();
        _sc.ShakyCameCustom(2, 0.2f);
        _lavaCollider.enabled = true;
        _isStartLava = true;
        yield return new WaitForSeconds(2.5f);
        _isStartLava = false;
        _rockFall.Stop();
    }

    private void PlayLavaSound()
    {
        if (!_lavaEventInstance.isValid())
        {
            _lavaEventInstance = RuntimeManager.CreateInstance(lavaSound);
            _lavaEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            _lavaEventInstance.start();
        }
    }

    private void PlayLavaBurntSound()
    {
        RuntimeManager.PlayOneShot(lavaBurntSound, transform.position);
    }

    public void StopLavaSound()
    {
        if (_lavaEventInstance.isValid())
        {
            _lavaEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _lavaEventInstance.release();
        }
    }

    private void OnDestroy()
    {
        StopLavaSound();
    }
}
