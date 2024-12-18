using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    private float _followSpeed = 100f;
    [SerializeField] Vector3 _offset = new Vector3(0, 1, 0);

    private void Start()
    {
        
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + _offset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            transform.rotation = Quaternion.identity;
        }
    }
}
