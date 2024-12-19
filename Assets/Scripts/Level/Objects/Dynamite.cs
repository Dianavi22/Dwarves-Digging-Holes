using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour, IGrabbable
{
    private Action throwOnDestroy;
    public GameObject spawnPoint;

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
        Spawn();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.Component.TryGetInParent<BigRock>(collision.collider, out var bigRock))
        {
            HandleDestroy();
            Destroy(spawnPoint);
            StartCoroutine(bigRock.DestroyBigRock());
        }
    }

    public void Spawn()
    {
        spawnPoint.GetComponent<SpawnDynamite>().SpawnTnt();
    }
}
