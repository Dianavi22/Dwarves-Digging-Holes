using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] prefabList;
    public float spawnDistance = 40f;
    public float despawnTime = 6f;
    public string triggerTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            Debug.Log("platform trigger");
            int randIndex = Random.Range(0, prefabList.Length);
            Instantiate(prefabList[randIndex], other.transform.root.position + new Vector3(spawnDistance, 0f, 0f), Quaternion.identity);
            Destroy(other.transform.root.gameObject, despawnTime);
        }
    }
}
