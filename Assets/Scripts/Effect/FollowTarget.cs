using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 _offset = new Vector3(0, 1, 0);
    [SerializeField] float _followSpeed = 100f;

    [SerializeField] List<Image> _images;
    [SerializeField] SpriteRenderer _circle;
    [SerializeField] ParticleSystem _openTutoPart;
    [SerializeField] private EventReference spawnBubbleSound;
    [SerializeField] private EventReference depopBubbleTutoSound;

    private bool _isOpen = false;

    Color alphaZero = new Color(0f, 0f, 0f, 0f);
    Color alpha = new Color(1f, 1f, 1f, 1f);

    private bool previousState = false;
    private float t;

    private Tuto _tuto;
    private void Start()
    {
        _tuto = GameManager.Instance.isInMainMenu ? default : TargetManager.Instance.GetGameObject<Tuto>();
        TotalClose(); 
    }

    void Update()
    {
        if (_isOpen != previousState && !GameManager.Instance.isEnding && !GameManager.Instance.isGameOver)
        {
            t = 0f;
            previousState = _isOpen;
        }

        if (t < 1f)
        {
            t += Time.deltaTime * 2;
            t = Mathf.Clamp01(t);
        }

        if (GameManager.Instance.isEnding || GameManager.Instance.isGameOver)
        {
            CloseTuto();
        }

        if (target != null)
        {
            Vector3 targetPosition = target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.identity;
        }


        if (GameManager.Instance.isGameStarted || _tuto.isInTuto)
        {
            Color lerp = _isOpen
                ? Color.Lerp(alphaZero, alpha, t)
                : Color.Lerp(alpha, alphaZero, t);
            SetImageAlphaColor(lerp);
        }
    }

    private void SetImageAlphaColor(Color color)
    {
        foreach (var image in _images)
        {
            image.color = color;
        }

        if (_circle != null)
        {
            _circle.color = color;
        }
    }

    public void TotalClose()
    {
        SetImageAlphaColor(alphaZero);
        _isOpen = false;
        previousState = false;
        t = 0f; 
    }

    public void OpenTuto()
    {
        if (!_isOpen)
        {
            _openTutoPart.Play();
            //SpawnBubbleSound(); 
            // ! Sound problem at the end of the game, I don't know why it's buggy (Tristan)
        }
        _isOpen = true;
    }

    public void CloseTuto()
    {
        _isOpen = false;
    }

    #region Sounds
    private void SpawnBubbleSound()
    {
        RuntimeManager.PlayOneShot(spawnBubbleSound, transform.position);
    }
    private void DepopBubbleTutoSound()
    {
        RuntimeManager.PlayOneShot(depopBubbleTutoSound, transform.position);
    }
    #endregion
}
