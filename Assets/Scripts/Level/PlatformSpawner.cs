using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabList;
    [SerializeField] GameObject[] prefabListTest;
    [SerializeField] string platformTrigger;
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool destroyOnTriggerExit = true;
    [SerializeField] float maximumDifficulty = 1.25f;

    [SerializeField] Image ProgressBar;
    public int platformCount = -1;  
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

    private void SetProgressBar()
    {
        ProgressBar.fillAmount = Mathf.Max(platformCount, (float)platformCount / GameManager.Instance.Difficulty.PlateformObjective);
    }

    public void SpawnPlatform()
    {
        if(platformCount == GameManager.Instance.Difficulty.PlateformObjective) {
            Debug.Log("end platform");
            //! Instantiate the end platform
            //Instantiate(, new Vector3(spawnPoint.transform.position.x + offset, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            return;

        }
        platformCount++;
        SetProgressBar();
        Debug.Log("SPAWN PLATFORM");
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

        Instantiate(prefabList[randIndex], new Vector3(spawnPoint.transform.position.x + 1, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity, gameObject.transform);
        currentDifficulty += selectedPlatform.blockDifficulty;
    }
}
