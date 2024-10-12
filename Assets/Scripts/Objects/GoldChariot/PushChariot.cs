using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PushChariot : MonoBehaviour
{
    [SerializeField] private float pushForce = 150;
    [SerializeField] private Rigidbody chariotRigidbody;
    [SerializeField] private EventReference chariotSound;
    private EventInstance _chariotEventInstance;
    private SoundUtils soundUtils;

    private bool _isTriggerActive = false;
    private PlayerFatigue _playerFatigue;
    private bool _isSoundPlaying = false;

    void Start()
    {
        soundUtils = gameObject.AddComponent<SoundUtils>();
    }

    private void FixedUpdate()
    {
        if (chariotRigidbody.velocity.x > 0)
        {
            PlayChariotSound();
        }
        else
        {
            PauseChariotSound();
        }

        if (!_isTriggerActive || _playerFatigue == null) return;

        if (_playerFatigue.ReduceCartsFatigue(10f * Time.deltaTime))
        {
            chariotRigidbody.AddForce(pushForce, 0, 0, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out var player) && !player.isHoldingObject && !_isTriggerActive)
        {
            _isTriggerActive = true;
            _playerFatigue = player.playerFatigue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out _) && _isTriggerActive)
        {
            _isTriggerActive = false;
            _playerFatigue = null;
        }
    }

    #region SFX
    private void PlayChariotSound()
    {
        if (!_isSoundPlaying)
        {
            if (!_chariotEventInstance.isValid())
            {
                _chariotEventInstance = RuntimeManager.CreateInstance(chariotSound);
                _chariotEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                _chariotEventInstance.start();
            }
            else
            {
                soundUtils.UnpauseWithFade(_chariotEventInstance, 0.1f);
            }
            
            _isSoundPlaying = true;
        }
    }

    public void PauseChariotSound()
    {
        if (_isSoundPlaying)
        {
            FMOD.RESULT result = _chariotEventInstance.getPaused(out bool isPaused);
            if (result == FMOD.RESULT.OK && !isPaused)
            {
                soundUtils.PauseWithFade(_chariotEventInstance, 0.1f);
                _isSoundPlaying = false;
            }
        }
    }

    public void StopChariotSound()
    {
        if (_chariotEventInstance.isValid())
        {
            _chariotEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _chariotEventInstance.release();
            _isSoundPlaying = false;
        }
    }
    #endregion
}
