using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<Player>(collision.collider, out var player))
        {
            player.HandleDestroy();
        }
        if (Utils.Component.TryGetInParent<GoldChariot>(collision.collider, out var goldChariot))
        {
            print("goldChariot");
        }
        Destroy(gameObject);
    }
}
