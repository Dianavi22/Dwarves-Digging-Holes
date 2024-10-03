using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool debugMode;
    public float scrollingSpeed; // Is used in Platform script
    [SerializeField] private PlatformSpawner blockSpawner;

    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;
  
    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private  GoldChariot _goldChariot;

    [HideInInspector]
    public List<GameObject> PickaxeInstanceList;
    public int MaxNbPickaxe = 1;

    [SerializeField] TMP_Text _textGameOverCondition;
    [SerializeField] string _goblinDeathCondition;
    [SerializeField] string _lavaDeathCondition;

    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        foreach (Pickaxe pickaxe in FindObjectsOfType<Pickaxe>()) 
            PickaxeInstanceList.Add(pickaxe.gameObject);
    }

    void Start()
    {
        if (debugMode) Debug.LogWarning("GAME MANAGER DEBUG MODE");
        
        GameStarted();
    }

    void Update()
    {
        if (_goldChariot.GoldCount <= 0)
        {
            GameOver(0);
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

    public void GameOver(int index)
    {
        if (debugMode) return;
        
        isGameOver = true;

        if (index == 0)
        {
            _textGameOverCondition.text = _goblinDeathCondition;
        }
        if (index == 1)
        {
            _textGameOverCondition.text = _lavaDeathCondition;
        }
        
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton);
        Time.timeScale = 0;
    }
}
