using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabList;
    [SerializeField] GameObject[] prefabListTest;
    [SerializeField] string platformTrigger;
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool destroyOnTriggerExit = true;
    [SerializeField] float maximumDifficulty = 1.25f;
    private float offset = 38; 

    private float currentDifficulty = 0;

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
        if(prefabList.Length == 1) {
            Instantiate(prefabList[0], new Vector3(spawnPoint.transform.position.x + offset, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            return;
        }

        Platform selectedPlatform;
        int randIndex;
        int iteration = 0;

        do
        {
            if(iteration == prefabList.Length) {
                currentDifficulty = 0;
            }

            randIndex = Random.Range(0, prefabList.Length);
            selectedPlatform = prefabList[randIndex].GetComponent<Platform>();
            iteration ++;
        }
        while ((selectedPlatform.blockDifficulty + currentDifficulty) >= maximumDifficulty + 0.2f);

        Instantiate(prefabList[randIndex], new Vector3(spawnPoint.transform.position.x + 2, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
        currentDifficulty += selectedPlatform.blockDifficulty;
    }
}
