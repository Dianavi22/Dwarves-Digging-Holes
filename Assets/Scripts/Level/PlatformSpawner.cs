using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabList;
    [SerializeField] GameObject[] prefabEndingList;
    [SerializeField] GameObject[] prefabListTest;
    [SerializeField] string platformTrigger;
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool destroyOnTriggerExit = true;
    [SerializeField] float maximumDifficulty = 1.25f;

    [SerializeField] Image ProgressBar;

    [SerializeField] Image InfiniteprogressBar;

    public int platformCount = 0;

    private float offset = 38;
    private float currentDifficulty = 0;
    private GameMode gameMode;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(prefabList.Length == 1) return;
        gameMode = (GameMode)PlayerPrefs.GetInt(Constant.MODE_KEY, 0);
        ProgressBar.transform.parent.gameObject.SetActive(gameMode == GameMode.Normal);
        InfiniteprogressBar.transform.parent.gameObject.SetActive(gameMode != GameMode.Normal);
    }

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

    //todo: Set (semi) Continous Fill
    private void SetProgressBar()
    {
        ProgressBar.fillAmount = platformCount / (float)GameManager.Instance.Difficulty.PlateformObjective;
    }

    public void SpawnPlatform()
    {
        if (prefabList.Length == 1)
        {
            Instantiate(prefabList[0], new Vector3(spawnPoint.transform.position.x + offset, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            return;
        }
        if (gameMode == GameMode.Normal)
        {
            if (platformCount == GameManager.Instance.Difficulty.PlateformObjective - 1)
            {
                platformCount++;
                SetProgressBar();
                //! Instantiate the end platform
                Instantiate(prefabEndingList[1], new Vector3(spawnPoint.transform.position.x + 1, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity, gameObject.transform);
                return;
            }

            if (platformCount == GameManager.Instance.Difficulty.PlateformObjective)
            {
                SetProgressBar();
                return;
            }

            platformCount++;
            SetProgressBar();
        }

        // if(platformCount == GameManager.Instance.Difficulty.PlateformObjective -1) {
        //     //Instantiate(prefabListTest[1], new Vector3(spawnPoint.transform.position.x + offset, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity, gameObject.transform);
        //     platformCount++;
        //     SetProgressBar();
        //     return;
        // }

        Platform selectedPlatform;
        int randIndex;
        int iteration = 0;

        do
        {
            if (iteration == prefabList.Length)
            {
                currentDifficulty = 0;
            }

            randIndex = Random.Range(0, prefabList.Length);
            selectedPlatform = prefabList[randIndex].GetComponent<Platform>();
            iteration++;
        }
        while ((selectedPlatform.blockDifficulty + currentDifficulty) >= maximumDifficulty + 0.2f);

        Instantiate(prefabList[randIndex], new Vector3(spawnPoint.transform.position.x +1, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity, gameObject.transform);
        currentDifficulty += selectedPlatform.blockDifficulty;
    }
}
