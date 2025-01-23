using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    public bool isRespawn = false;
    [SerializeField] Image _arrow;
    void Update()
    {
        if (isRespawn)
        {
            _arrow.enabled = true;
            float rotation = rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotation);
        }
        else
        {
            _arrow.enabled = false;
        }

    }
}