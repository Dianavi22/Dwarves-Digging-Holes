using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    public bool isRespawn = false;
    public bool isImpossibleRespawn = false;
    [SerializeField] Image _arrow;
    [SerializeField] Image _impossibleArrow;
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

        if (isImpossibleRespawn)
        {
            _impossibleArrow.enabled = true;
        }
        else
        {
            _impossibleArrow.enabled = false;

        }

    }
}