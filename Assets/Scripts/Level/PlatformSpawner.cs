using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] prefabList;
    public string platformTrigger;
    public Transform spawnPoint;
    [SerializeField] bool destroyOnTriggerExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(platformTrigger))
            SpawnPlatform();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(platformTrigger) && destroyOnTriggerExit)
            Destroy(other.transform.parent.gameObject);
    }

    public void SpawnPlatform()
    {
        int randIndex = Random.Range(0, prefabList.Length);
        Instantiate(prefabList[randIndex], spawnPoint.position, Quaternion.identity);
    }
}
