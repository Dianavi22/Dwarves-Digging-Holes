using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlatformSpawner platformSpawner;

    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private GameObject _GameOverCanvas;

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
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
        print("GameOver");
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);

    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
