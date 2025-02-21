using System.Collections;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class GameOST : MonoBehaviour
{
    public static GameOST Instance { get; private set; }

    [SerializeField] private EventReference ostSound;
    private EventInstance _ostEventInstance;
    private bool _isPlaying = false;
    private Coroutine _fadeCoroutine;
    private float _fadeDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _ostEventInstance = RuntimeManager.CreateInstance(ostSound);
        _ostEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    public void StartMusic() //Call in Lava script
    {
        if (!_isPlaying)
        {
            _ostEventInstance.start();
            _isPlaying = true;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeVolume(0f, 1f, _fadeDuration));
        }
    }

    public void StopMusic()
    {
        if (_ostEventInstance.isValid())
        {
            _ostEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _ostEventInstance.release();
            _isPlaying = false;
        }
    }

    public void PauseMusic()
    {
        if (_isPlaying)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeVolume(1f, 0f, _fadeDuration, () =>
            {
                _ostEventInstance.setPaused(true);
                _isPlaying = false;
            }));
        }
    }

    public void ResumeMusic()
    {
        if (!_isPlaying && _ostEventInstance.isValid())
        {
            _ostEventInstance.setPaused(false);
            _isPlaying = true;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeVolume(0f, 1f, _fadeDuration));
        }
    }

    private IEnumerator FadeVolume(float start, float end, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float volume = Mathf.Lerp(start, end, elapsed / duration);
            _ostEventInstance.setParameterByName("Volume", volume);
            yield return null;
        }

        _ostEventInstance.setParameterByName("Volume", end);
        onComplete?.Invoke();
    }

    public void SetMusicTime(float minutes, float seconds)
    {
        if (_ostEventInstance.isValid())
        {
            int milliseconds = Mathf.RoundToInt((minutes * 60 + seconds) * 1000);
            _ostEventInstance.setTimelinePosition(milliseconds);
        }
        else
        {
            Debug.LogWarning("GameOST: Impossible de modifier la position de la musique, EventInstance invalide.");
        }
    }

    public void PauseAndSetMusicTime(float minutes, float seconds)
    {
        if (_isPlaying)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeVolume(1f, 0f, _fadeDuration, () =>
            {
                _ostEventInstance.setPaused(true);
                _isPlaying = false;
                SetMusicTime(minutes, seconds); 
            }));
        }
    }

    private void OnDestroy()
    {
        if (_ostEventInstance.isValid())
        {
            _ostEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _ostEventInstance.release();
        }
    }
}
