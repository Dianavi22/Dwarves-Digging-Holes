using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private float distanceFromPlayer;

    public Transform playerToFollow;

    // Update is called once per frame
    void Update()
    {
        if(playerToFollow) {
            transform.position = new(playerToFollow.position.x, playerToFollow.position.y + 0.25f, distanceFromPlayer);
        }
        
    }
}
