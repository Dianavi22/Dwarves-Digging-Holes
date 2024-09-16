using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public float spawnDistance = 40f;
    public float despawnTime = 6f;

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("platform trigger");
        Instantiate(platformPrefab, other.transform.root.position + new Vector3(spawnDistance, 0f, 0f), Quaternion.identity);
        Destroy(other.transform.root.gameObject, despawnTime);
    }
}
