using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeadFatigueBar : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1f, 0);

    public CanvasGroup cartsFatigueCanvasGroup;
    public CanvasGroup miningFatigueCanvasGroup;

    public Image cartsFatigueBar;
    public Image miningFatigueBar;

    public TMP_Text cartsFatigueText;
    public TMP_Text miningFatigueText;

    private Player _player;
    private Camera _mainCamera;

    public float displayThreshold = 0.5f;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (cartsFatigueCanvasGroup != null)
        {
            cartsFatigueCanvasGroup.alpha = 0f;
            cartsFatigueCanvasGroup.gameObject.SetActive(false);
        }

        if (miningFatigueCanvasGroup != null)
        {
            miningFatigueCanvasGroup.alpha = 0f;
            miningFatigueCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void Initialize(Player player)
    {
        _player = player;

        if (_player.GetFatigue() != null)
        {
            _player.GetFatigue().onCartsFatigueChanged.AddListener(UpdateCartsFatigueUI);
            _player.GetFatigue().onMiningFatigueChanged.AddListener(UpdateMiningFatigueUI);

            UpdateCartsFatigueUI(_player.GetFatigue().currentCartsFatigue, _player.GetFatigue().maxCartsFatigue);
            UpdateMiningFatigueUI(_player.GetFatigue().currentMiningFatigue, _player.GetFatigue().maxMiningFatigue);
        }
    }

    private void OnDestroy()
    {
        if (_player != null && _player.GetFatigue() != null)
        {
            _player.GetFatigue().onCartsFatigueChanged.RemoveListener(UpdateCartsFatigueUI);
            _player.GetFatigue().onMiningFatigueChanged.RemoveListener(UpdateMiningFatigueUI);
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            Vector3 worldPosition = _player.transform.position + offset;
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
            transform.position = screenPosition;
        }
    }

    public void UpdateCartsFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (cartsFatigueBar != null)
        {
            UpdateBar(currentFatigue, maxFatigue, cartsFatigueBar, cartsFatigueText);


            float ratio = currentFatigue / maxFatigue;

            if (ratio < displayThreshold)
            {
                if (cartsFatigueCanvasGroup != null && cartsFatigueCanvasGroup.alpha == 0f)
                {
                    cartsFatigueCanvasGroup.gameObject.SetActive(true);
                    StopCoroutine("FadeOutCartsFatigue");
                    StartCoroutine("FadeInCartsFatigue");
                }
            }
            else
            {
                if (cartsFatigueCanvasGroup != null && cartsFatigueCanvasGroup.alpha == 1f)
                {
                    StopCoroutine("FadeInCartsFatigue");
                    StartCoroutine("FadeOutCartsFatigue");
                }
            }
        }
    }

    public void UpdateMiningFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (miningFatigueBar != null)
        {
            UpdateBar(currentFatigue, maxFatigue, miningFatigueBar, miningFatigueText);

            float ratio = currentFatigue / maxFatigue;

            if (ratio < displayThreshold)
            {
                if (miningFatigueCanvasGroup != null && miningFatigueCanvasGroup.alpha == 0f)
                {
                    miningFatigueCanvasGroup.gameObject.SetActive(true);
                    StopCoroutine("FadeOutMiningFatigue");
                    StartCoroutine("FadeInMiningFatigue");
                }
            }
            else
            {
                if (miningFatigueCanvasGroup != null && miningFatigueCanvasGroup.alpha == 1f)
                {
                    StopCoroutine("FadeInMiningFatigue");
                    StartCoroutine("FadeOutMiningFatigue");
                }
            }
        }
    }

    public void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text)
    {
        float ratio = currentValue / maxValue;
        //bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, ratio, Time.deltaTime * maxValue);
        bar.fillAmount = ratio;
        text.text = ((int)currentValue).ToString();
    }



    private IEnumerator FadeInCartsFatigue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cartsFatigueCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOutCartsFatigue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cartsFatigueCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        cartsFatigueCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeInMiningFatigue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            miningFatigueCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOutMiningFatigue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            miningFatigueCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        miningFatigueCanvasGroup.gameObject.SetActive(false);
    }
}