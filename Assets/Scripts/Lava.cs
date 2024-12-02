using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private Collider _lavaCollider;
    [SerializeField] private EventReference lavaSound;
    [SerializeField] private EventReference lavaBurntSound;
    private EventInstance _lavaEventInstance;

    private void Start()
    {
        _lavaCollider.enabled = false;
        Invoke(nameof(CooldownLava), 4);
        PlayLavaSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<IGrabbable>(other, out var grabbable))
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

        if (Utils.TryGetParentComponent<Rock>(other, out var rock))
        {
            Destroy(rock.gameObject);
        }
    }

    private void CooldownLava()
    {
        _lavaCollider.enabled = true;
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
