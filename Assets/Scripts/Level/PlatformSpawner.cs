using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] prefabList;
    public float despawnTime = 6f;
    public float platformSpeed = 3f;
    public string triggerTag;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SpawnPlatform();
            Destroy(other.transform.root.gameObject, despawnTime);
        }
    }

    public void SpawnPlatform()
    {
        int randIndex = Random.Range(0, prefabList.Length);
        var instance = Instantiate(prefabList[randIndex], spawnPoint.position, Quaternion.identity);
        var platformScript = instance.GetComponent<Platform>();
        if (platformScript) platformScript.movementSpeed = platformSpeed;
    }
}
