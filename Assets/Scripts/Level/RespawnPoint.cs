using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] float radiusDetectCollider = 1f;
    [SerializeField] LayerMask _ignoreLayer;

    public GameObject circle;
    
    private bool IsReadyToRespawn, DelayRespawn;
    private Queue<Player> respawnQueue;

    private void Start()
    {
        respawnQueue = new Queue<Player>();
    }

    void Update()
    {
        var colliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, 0), radiusDetectCollider, ~_ignoreLayer);

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

    public void AddToRespawnQueue(Player player)
    {
        respawnQueue.Enqueue(player);
    }

    private bool IsNexToRespawn(Player player)
    {
        return respawnQueue.Count > 0 && respawnQueue.Peek() == player;
    }

    public bool CheckRespawnSequence(Player player)
    {
        if (IsReadyToRespawn && IsNexToRespawn(player) && DelayRespawn == false)
        {
            respawnQueue.Dequeue();
            DelayRespawn = true;
            
            DOVirtual.DelayedCall(1.5f, () =>
            {
                DelayRespawn = false;
            });

            return true;
        }

        return false;
    }
}
