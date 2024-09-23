using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] prefabList;
    public float despawnTime = 6f;
    public string plateformTrigger;
    public Transform spawnPoint;
    [SerializeField] bool destroyOnTriggerExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(plateformTrigger))
            SpawnPlatform();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(plateformTrigger) && destroyOnTriggerExit)
            Destroy(other.transform.parent.gameObject);
    }

    public void SpawnPlatform()
    {
        int randIndex = Random.Range(0, prefabList.Length);
        Instantiate(prefabList[randIndex], spawnPoint.position, Quaternion.identity);
    }
}
