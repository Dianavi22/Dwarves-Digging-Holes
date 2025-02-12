using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Dynamite : MonoBehaviour, IGrabbable
{
    [SerializeField] private EventReference explosionSound;

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
            ExplosionSound(gameObject.transform.position);
            HandleDestroy();
            Destroy(spawnPoint);
            StartCoroutine(bigRock.DestroyBigRock());
        }
    }

    public void Spawn()
    {
        spawnPoint.GetComponent<SpawnDynamite>().SpawnTnt();
    }

    private void ExplosionSound(Vector3 position)
    {
        RuntimeManager.PlayOneShot(explosionSound, position);
    }
}
