using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using System;

public class PlayerHeadFatigueBar : MonoBehaviour
{
    [Header("UI Offset")]
    [SerializeField] private Vector3 offset = new(0, 1f, 0);

    [Header("CanvasGroups")]
    [SerializeField] private CanvasGroup cartsFatigueCanvasGroup;
    [SerializeField] private CanvasGroup miningFatigueCanvasGroup;

    [Header("Fatigue Bars")]
    [SerializeField] private Image cartsFatigueBar;
    [SerializeField] private Image miningFatigueBar;

    [Header("Fatigue Texts")]
    [SerializeField] private TMP_Text cartsFatigueText;
    [SerializeField] private TMP_Text miningFatigueText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    private const float DISPLAY_THRESHOLD = 0.5f;
    private const float CRITICAL_THRESHOLD = 0.2f;

    [SerializeField] private float blinkDuration = 0.15f;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color warningColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color criticalColor = Color.red;

    [SerializeField] ParticleSystem _miningPart;
    [SerializeField] ParticleSystem _cartPart;

    private bool isBlinkingCarts = false;
    private bool isBlinkingMining = false;

    private Player _player;

    private void Awake()
    {
        if (cartsFatigueCanvasGroup != null)
        {
            cartsFatigueCanvasGroup.gameObject.SetActive(false);
        }

        if (miningFatigueCanvasGroup != null)
        {
            miningFatigueCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void Initialize(Player player)
    {
        _player = player;

        Invoke(nameof(DelayedInitialize), 2f);
    }

    private void DelayedInitialize()
    {
        PlayerFatigue _f = _player.GetFatigue();
        if (_f != null)
        {
            _f.onCartsFatigueChanged.AddListener(UpdateCartsFatigueUI);
            _f.onMiningFatigueChanged.AddListener(UpdateMiningFatigueUI);

            UpdateCartsFatigueUI(_f.currentCartsFatigue, _f.MaxPushCartFatigue);
            UpdateMiningFatigueUI(_f.currentMiningFatigue, _f.MaxMiningFatigue);
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
        if (_player == null) return;

        Vector3 worldPosition = _player.transform.position + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition;

        PlayerFatigue _f = _player.GetFatigue();

        if (!GameManager.Instance.isInMainMenu && !GameManager.Instance.isEnding && !GameManager.Instance.isGameOver)
        {
            if (cartsFatigueBar != null) ChangeBarColor(cartsFatigueBar, _f.currentCartsFatigue / _f.MaxPushCartFatigue);
            if (miningFatigueBar != null) ChangeBarColor(miningFatigueBar, _f.currentMiningFatigue / _f.MaxMiningFatigue);
        }
        
    }

    public void UpdateCartsFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (cartsFatigueBar == null) return;

        UpdateBar(currentFatigue, maxFatigue, cartsFatigueBar, cartsFatigueText);
        float ratio = currentFatigue / maxFatigue;

        if (ratio < DISPLAY_THRESHOLD)
        {
            if (!cartsFatigueCanvasGroup.isActiveAndEnabled)
            {
                StartCoroutine(Anim.FadeIn(fadeDuration, cartsFatigueCanvasGroup));
            }

            if (ratio < CRITICAL_THRESHOLD)
            {
                if (!isBlinkingCarts)
                {
                    isBlinkingCarts = true;
                    StartCoroutine(BlinkCartsFatigue());
                }
            }
            else if (isBlinkingCarts)
            {
                isBlinkingCarts = false;
            }
        }
        else if (cartsFatigueCanvasGroup.isActiveAndEnabled)
        {
            StartCoroutine(Anim.FadeOut(fadeDuration, cartsFatigueCanvasGroup));
        }
    }

    public void UpdateMiningFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (miningFatigueBar == null) return;

        UpdateBar(currentFatigue, maxFatigue, miningFatigueBar, miningFatigueText);
        float ratio = currentFatigue / maxFatigue;

        if (ratio < DISPLAY_THRESHOLD && _player.GetHealth().IsAlive)
        {
            if (!miningFatigueCanvasGroup.isActiveAndEnabled)
            {
                StartCoroutine(Anim.FadeIn(fadeDuration, miningFatigueCanvasGroup));
            }

            if (ratio < CRITICAL_THRESHOLD)
            {
                if (!isBlinkingMining)
                {
                    isBlinkingMining = true;
                    StartCoroutine(BlinkMiningFatigue());
                }
            }
            else if (isBlinkingMining )
            {
                isBlinkingMining  = false;
            }
        }
        else if (miningFatigueCanvasGroup.isActiveAndEnabled)
        {
            StartCoroutine(Anim.FadeOut(fadeDuration, miningFatigueCanvasGroup));
        }
    }

    private static void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text)
    {
        float ratio = currentValue / maxValue;
        bar.fillAmount = ratio;
        text.text = ((int)currentValue).ToString();
    }

    private void ChangeBarColor(Image bar, float ratio)
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

        ParticleSystem.MainModule main = _cartPart.GetComponent<ParticleSystem>().main;
        main.startColor = bar.color;

        ParticleSystem.MainModule main2 = _miningPart.GetComponent<ParticleSystem>().main;
        main2.startColor = bar.color;
    }
    private IEnumerator BlinkCartsFatigue()
    {
        isBlinkingCarts = true;
        while (isBlinkingCarts)
        {
            cartsFatigueCanvasGroup.alpha = Mathf.PingPong(Time.time * (1f / blinkDuration), 1f);
            yield return null;
        }
        cartsFatigueCanvasGroup.alpha = 1f;
    }

    private IEnumerator BlinkMiningFatigue()
    {
        isBlinkingMining = true;
        while (isBlinkingMining)
        {
            miningFatigueCanvasGroup.alpha = Mathf.PingPong(Time.time * (1f / blinkDuration), 1f);
            yield return null;
        }
        miningFatigueCanvasGroup.alpha = 1f;
    }
}