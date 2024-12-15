using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour, IGrabbable
{
    private Action throwOnDestroy;
  
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        PlayerActions actions = currentPlayer.GetActions();
    }

    public void HandleDestroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<BigRock>(collision.collider, out var bigRock))
        {
            HandleDestroy();
            bigRock.DestroyBigRock();
        }
    }
}
