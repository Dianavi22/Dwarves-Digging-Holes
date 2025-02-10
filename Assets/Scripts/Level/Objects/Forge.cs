using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Forge : MonoBehaviour
{
    [SerializeField] private Pickaxe pickaxePrefab;
    private GameManager _gameManager;
    [SerializeField] private GameObject _bubblePickaxe;
    [SerializeField] private EventManager _eventManager;
    [SerializeField] private ParticleSystem _forgePart;
    [SerializeField] private Image _loadImage;

    private bool _isCreatingPickaxe = false;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (_gameManager.CanCreatePickaxe && !_eventManager.isForgeEvent && !_isCreatingPickaxe && !_gameManager.isGameOver)
        {
            _bubblePickaxe.SetActive(true);
        }
        else
        {
            _bubblePickaxe.SetActive(false);
        }
    }

    public void BuildPickaxe()
    {
        var _createdPickaxe = Instantiate(pickaxePrefab, transform.position, Quaternion.identity);
        _gameManager.AddPickaxe(_createdPickaxe);
    }

    public IEnumerator LoadPickaxe(bool reverse = false)
    {
        float startFillAmount = _loadImage.fillAmount;
        if (_eventManager.isForgeEvent)
        {
            _loadImage.fillAmount = 0;
            _isCreatingPickaxe = false;
            yield return null;
        }
        else
        {
            if (!_forgePart.isPlaying)
            {
                _forgePart.Play();

            }
            _isCreatingPickaxe = true;

            float duration = Mathf.Abs(_loadImage.fillAmount - (reverse ? 0f : 1f));
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _loadImage.fillAmount = reverse ? Mathf.Lerp(startFillAmount, 0f, elapsed / duration) : Mathf.Lerp(startFillAmount, 1f, elapsed / duration);
                yield return null;
            }
            _loadImage.fillAmount = reverse ? 0f : 1f;

            if (!reverse)
            {
                BuildPickaxe();
            }
            _isCreatingPickaxe = false;
            _forgePart.Stop();
        }

    }



}
