using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public PlatformSpawner platformSpawner;
    public float scrollingSpeed;

    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private GameObject _GameOverCanvas;
  
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
        Invoke("InitPlatformSpawner", 3f);
    }

    private void InitPlatformSpawner()
    {
        platformSpawner.SpawnPlatform();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);

    }



}
