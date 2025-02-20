using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private Light _lavaLight;
    [SerializeField] private Collider _lavaCollider;
    [SerializeField] private EventReference lavaSound;
    [SerializeField] private EventReference lavaBurntSound;
    [SerializeField] private EventReference lavaEruptionSound;
    [SerializeField] ParticleSystem _rockFall;
    [SerializeField] ParticleSystem _lavaSpawn;
    [SerializeField] GameObject _tutoBubble;

    private bool _isCoolDown = true;
    private EventInstance _lavaEventInstance;
    private bool _isStartLava;
    private GameObject preventDoubleCall = null;

    private void Start()
    {
        _lavaCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if( !GameManager.Instance.isChariotWin && !GameManager.Instance.isEnding && !GameManager.Instance.isGameOver && !GameManager.Instance.isEnding)
        {
            if (Utils.Component.TryGetInParent<IGrabbable>(other, out var grabbable))
            {
                if(preventDoubleCall == grabbable.GetGameObject()) return;

                preventDoubleCall = grabbable.GetGameObject();
                PlayLavaBurntSound();
                grabbable.HandleDestroy();

                Invoke(nameof(ResetPreventDoubleCall), 0.5f);
            }

            if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
            {
                StartCoroutine(rock.DestroyRockByLava());
            }
        }
      
    }
    private void FixedUpdate()
    {
        if (_isStartLava)
        {
            transform.Translate(Vector3.right * 5f * Time.fixedDeltaTime * 0.5f);
        }
    }


    public IEnumerator CooldownLava()
    {
        if (_isCoolDown)
        {
            _lavaSpawn.Play();
            yield return new WaitForSeconds(1.5f);
            TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(2, 0.2f);

            _lavaLight.DOIntensity(4f, 2f);
            _rockFall.Play();
            PlayLavaEruptionSound();
            
            _lavaCollider.enabled = true;
            _isStartLava = true;
            yield return new WaitForSeconds(2.5f);
            _isStartLava = false;
            _rockFall.Stop();
            _lavaSpawn.Stop();
            _isCoolDown = false;
            
            yield return new WaitForSeconds(1f);
            TargetManager.Instance.GetGameObject<StudioEventEmitter>().gameObject.SetActive(true);
        }
    }

    private void ResetPreventDoubleCall()
    {
        preventDoubleCall = null;
    }

    #region Sound
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

    private void PlayLavaEruptionSound()
    {
        RuntimeManager.PlayOneShot(lavaEruptionSound, transform.position);
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

    #endregion
    
}
