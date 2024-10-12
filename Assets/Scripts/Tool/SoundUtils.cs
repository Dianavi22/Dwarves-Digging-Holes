using System.Collections;
using UnityEngine;

public class SoundUtils : MonoBehaviour
{
    private IEnumerator FadeEvent(FMOD.Studio.EventInstance eventInstance, float targetVolume, float duration, bool pauseAfterFade)
    {
        if (eventInstance.getVolume(out float currentVolume) == FMOD.RESULT.OK)
        {
            float startVolume = currentVolume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                eventInstance.setVolume(newVolume);
                yield return null;
            }

            eventInstance.setVolume(targetVolume);

            if (pauseAfterFade)
            {
                eventInstance.setPaused(true);
            }
        }
        else
        {
            Debug.LogError("Failed to get the current volume.");
        }
    }

    public void PauseWithFade(FMOD.Studio.EventInstance eventInstance, float fadeDuration)
    {
        if (eventInstance.getPaused(out bool isPaused) == FMOD.RESULT.OK && !isPaused)
        {
            StartCoroutine(FadeEvent(eventInstance, 0f, fadeDuration, true));
        }
        else
        {
            Debug.LogError("Failed to get the paused state or event is already paused.");
        }
    }

    public void UnpauseWithFade(FMOD.Studio.EventInstance eventInstance, float fadeDuration)
    {
        if (eventInstance.setPaused(false) == FMOD.RESULT.OK)
        {
            StartCoroutine(FadeEvent(eventInstance, 1f, fadeDuration, false));
        }
        else
        {
            Debug.LogError("Failed to unpause the event.");
        }
    }
}
