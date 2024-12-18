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

    Color alphaZero = new Color(1f, 1f, 1f, 0f);
    Color alpha = new Color(1f, 1f, 1f, 1f);

    private void Start()
    {
        CloseTuto();
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + _offset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            transform.rotation = Quaternion.identity;
        }

        if (_isOpen)
        {
            print("Open");
            for (int i = 0; i < _images.Count; i++)
            {
                _images[i].GetComponent<Image>().color = Color.Lerp(alphaZero, alpha, .2f);
            }
            _circle.GetComponent<SpriteRenderer>().color = Color.Lerp(alphaZero, alpha, .2f);
        }
        else
        {
            print("Close");

            for (int i = 0; i < _images.Count; i++)
            {
                _images[i].GetComponent<Image>().color = Color.Lerp(alpha, alphaZero, .2f);
            }
            _circle.GetComponent<SpriteRenderer>().color = Color.Lerp(alpha, alphaZero, .2f);
        }

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
