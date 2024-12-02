using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHeadFatigueBar : MonoBehaviour
{
    [Header("UI Offset")]
    public Vector3 offset = new Vector3(0, 1f, 0);

    [Header("CanvasGroups")]
    public CanvasGroup cartsFatigueCanvasGroup;
    public CanvasGroup miningFatigueCanvasGroup;

    [Header("Fatigue Bars")]
    public Image cartsFatigueBar;
    public Image miningFatigueBar;

    [Header("Fatigue Texts")]
    public TMP_Text cartsFatigueText;
    public TMP_Text miningFatigueText;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    private const float DISPLAY_THRESHOLD = 0.5f;
    private const float CRITICAL_THRESHOLD = 0.2f;

    [Header("Color Settings")]
    public Color normalColor = Color.blue;
    public Color warningColor = new Color(1f, 0.5f, 0f);
    public Color criticalColor = Color.red;

    [Header("Blink Settings")]
    private float blinkInterval = 0.3f;
    private bool isBlinkingCarts = false;
    private bool isBlinkingMining = false;

    private Player _player;
    private Camera _mainCamera;

    private bool isInitializationDelay = true;

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

        StartCoroutine(DelayedInitialize(2f));
    }

    private IEnumerator DelayedInitialize(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInitializationDelay = false;

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

            float cartsRatio = _player.GetFatigue().currentCartsFatigue / _player.GetFatigue().maxCartsFatigue;
            ChangeBarColor(cartsFatigueBar, cartsRatio);

            float miningRatio = _player.GetFatigue().currentMiningFatigue / _player.GetFatigue().maxMiningFatigue;
            ChangeBarColor(miningFatigueBar, miningRatio);

        }
    }

    public void UpdateCartsFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (cartsFatigueBar != null)
        {
            UpdateBar(currentFatigue, maxFatigue, cartsFatigueBar, cartsFatigueText);

            float ratio = currentFatigue / maxFatigue;

            if (ratio < DISPLAY_THRESHOLD)
            {
                if (cartsFatigueCanvasGroup != null && cartsFatigueCanvasGroup.alpha == 0f)
                {
                    cartsFatigueCanvasGroup.gameObject.SetActive(true);
                    StopCoroutine("FadeOutCartsFatigue");
                    StartCoroutine("FadeInCartsFatigue");
                }

                if (ratio < CRITICAL_THRESHOLD)
                {
                    if (!isBlinkingCarts)
                    {
                        StartCoroutine(BlinkCartsFatigue());
                    }
                }
                else if (isBlinkingCarts)
                {
                    isBlinkingCarts = false;
                    StopCoroutine(BlinkCartsFatigue());
                    cartsFatigueCanvasGroup.alpha = 1f;
                }
            }
            else
            {
                if (cartsFatigueCanvasGroup != null && cartsFatigueCanvasGroup.alpha == 1f)
                {
                    StopCoroutine("FadeInCartsFatigue");
                    StartCoroutine("FadeOutCartsFatigue");
                }

                if (isBlinkingCarts)
                {
                    isBlinkingCarts = false;
                    StopCoroutine(BlinkCartsFatigue());
                    cartsFatigueCanvasGroup.alpha = 1f;
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

            if (ratio < DISPLAY_THRESHOLD)
            {
                if (miningFatigueCanvasGroup != null && miningFatigueCanvasGroup.alpha == 0f)
                {
                    miningFatigueCanvasGroup.gameObject.SetActive(true);
                    StopCoroutine("FadeOutMiningFatigue");
                    StartCoroutine("FadeInMiningFatigue");
                }

                if (ratio < CRITICAL_THRESHOLD)
                {
                    if (!isBlinkingMining)
                    {
                        StartCoroutine(BlinkMiningFatigue());
                    }
                }
                else if (isBlinkingMining)
                {
                    isBlinkingMining = false;
                    StopCoroutine(BlinkMiningFatigue());
                    miningFatigueCanvasGroup.alpha = 1f;
                }
            }
            else
            {
                if (miningFatigueCanvasGroup != null && miningFatigueCanvasGroup.alpha == 1f)
                {
                    StopCoroutine("FadeInMiningFatigue");
                    StartCoroutine("FadeOutMiningFatigue");
                }

                if (isBlinkingMining)
                {
                    isBlinkingMining = false;
                    StopCoroutine(BlinkMiningFatigue());
                    miningFatigueCanvasGroup.alpha = 1f;
                }
            }
        }
    }

    public void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text)
    {
        float ratio = currentValue / maxValue;
        bar.fillAmount = ratio;
        text.text = ((int)currentValue).ToString();
    }

    private void ChangeBarColor(Image bar, float ratio)
    {
        if (bar != null)
        {
            if (ratio < CRITICAL_THRESHOLD)
            {
                bar.color = Color.Lerp(bar.color, criticalColor, Time.deltaTime * 5f);
            }
            else if (ratio < DISPLAY_THRESHOLD)
            {
                bar.color = Color.Lerp(bar.color, warningColor, Time.deltaTime * 5f);
            }
            else
            {
                bar.color = Color.Lerp(bar.color, normalColor, Time.deltaTime * 5f);
            }
        }
    }

    private IEnumerator BlinkCartsFatigue()
    {
        isBlinkingCarts = true;
        while (isBlinkingCarts)
        {
            cartsFatigueCanvasGroup.alpha = Mathf.PingPong(Time.time * (1f / blinkInterval), 1f);
            yield return null;
        }
        cartsFatigueCanvasGroup.alpha = 1f;
    }

    private IEnumerator BlinkMiningFatigue()
    {
        isBlinkingMining = true;
        while (isBlinkingMining)
        {
            miningFatigueCanvasGroup.alpha = Mathf.PingPong(Time.time * (1f / blinkInterval), 1f);
            yield return null;
        }
        miningFatigueCanvasGroup.alpha = 1f;
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
