using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    private float _followSpeed = 100f;
    [SerializeField] Vector3 _offset = new Vector3(0, 1, 0);

    [SerializeField] List<GameObject> _images;
    [SerializeField] GameObject _circle;

    private bool _isOpen = false;

    Color alphaZero = new Color(0f, 0f, 0f, 0f);
    Color alpha = new Color(1f, 1f, 1f, 1f);

    private bool previousState = false;
    private float t;

    private Tuto _tuto;
    private void Start()
    {
        _tuto = FindObjectOfType<Tuto>();
        TotalClose(); // Initialisation à l'état totalement fermé
    }

    void Update()
    {
        if (_isOpen != previousState)
        {
            t = 0f;
            previousState = _isOpen;
        }

        if (t < 1f)
        {
            t += Time.deltaTime * 2;
            t = Mathf.Clamp01(t);
        }

        if (target != null)
        {
            Vector3 targetPosition = target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.identity;
        }
        if(GameManager.Instance.isGameStarted || _tuto.isInTuto)
        {
            if (_isOpen)
            {
                foreach (var image in _images)
                {
                    var img = image.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = Color.Lerp(alphaZero, alpha, t);
                    }
                }

                if (_circle != null)
                {
                    var spriteRenderer = _circle.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.Lerp(alphaZero, alpha, t);
                    }
                }
            }
            else
            {
                foreach (var image in _images)
                {
                    var img = image.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = Color.Lerp(alpha, alphaZero, t);
                    }
                }

                if (_circle != null)
                {
                    var spriteRenderer = _circle.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.Lerp(alpha, alphaZero, t);
                    }
                }
            }
        }

        
    }

    public void TotalClose()
    {
        // Forcer toutes les couleurs à alphaZero immédiatement
        foreach (var image in _images)
        {
            var img = image.GetComponent<Image>();
            if (img != null)
            {
                img.color = alphaZero;
            }
        }

        if (_circle != null)
        {
            var spriteRenderer = _circle.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = alphaZero;
            }
        }

        // Réinitialiser les variables d'état
        _isOpen = false;
        previousState = false;
        t = 0f; // Assurer qu'il n'y a pas d'interpolation active
    }

    public void OpenTuto()
    {
        _isOpen = true;
    }

    public void CloseTuto()
    {
        _isOpen = false;
    }
}
