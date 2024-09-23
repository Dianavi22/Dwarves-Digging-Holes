using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlatformSpawner blockSpawner;

    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private GameObject _GameOverCanvas;
    //[SerializeField] private EventSystem _eventSystem;
    [SerializeField] GameObject _retryButton;
  
    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);
    }



    void Start()
    {
        GameStarted();

    }

    void Update()
    {
       
    }

    private void GameStarted()
    {
        _GameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        blockSpawner = GameObject.Find("BlockSpawner").GetComponent<PlatformSpawner>();
        Invoke("InitPlatformSpawner", 3f);
    }

    private void InitPlatformSpawner()
    {
        blockSpawner.SpawnPlatform();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton);


    }



}
