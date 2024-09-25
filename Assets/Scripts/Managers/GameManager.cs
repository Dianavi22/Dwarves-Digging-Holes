using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public bool debugMode;
    public float scrollingSpeed; // Is used in Platform script
    [SerializeField] private PlatformSpawner blockSpawner;

    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;
  
    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private  GoldChariot _goldChariot;

    public GameObject Pickaxe;

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        Pickaxe = GameObject.Find("Pickaxe");
    }

    void Start()
    {
        if (debugMode) Debug.LogWarning("GAME MANAGER DEBUG MODE");
        
        GameStarted();
    }

    void Update()
    {
        if (_goldChariot.goldCount <= 0)
        {
            GameOver();
        }
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
        if (debugMode) return;
        
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton);
        Time.timeScale = 0;
    }
}
