using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] prefabList;
    public float despawnTime = 6f;
    public string triggerTag;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SpawnPlatform();
            Destroy(other.transform.parent.gameObject, despawnTime);
        }
    }

    public void SpawnPlatform()
    {
        int randIndex = Random.Range(0, prefabList.Length);
        Instantiate(prefabList[randIndex], spawnPoint.position, Quaternion.identity);
    }
}
