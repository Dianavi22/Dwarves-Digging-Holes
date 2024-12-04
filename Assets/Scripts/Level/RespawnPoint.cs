using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] float radiusDetectCollider = 1f;

    public GameObject circle;
    public bool IsReadyToRespawn { private set; get; }

    void Update()
    {
        var colliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, 0), radiusDetectCollider);

        if (colliders.Length != 0)
        {
            foreach (var collider in colliders)
            {
                if (!collider.isTrigger)
                {
                    IsReadyToRespawn = false;
                }
                else
                {
                    IsReadyToRespawn = true;
                }
            }
        }
        else
        {
            IsReadyToRespawn = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, 0), radiusDetectCollider);
    }
}
