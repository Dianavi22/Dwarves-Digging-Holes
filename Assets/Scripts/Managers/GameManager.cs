using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool debugMode;
    public float scrollingSpeed; // Is used in Platform script
    [SerializeField] private PlatformSpawner blockSpawner;

    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;
  
    public static GameManager Instance; // A static reference to the GameManager instance

    [HideInInspector] public List<GameObject> PickaxeInstanceList;
    public int MaxNbPickaxe;

    [SerializeField] TMP_Text _textGameOverCondition;

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
        GameObject goldChariot = TargetManager.Instance.GetGameObject(Target.GoldChariot);
        if (goldChariot.GetComponent<GoldChariot>().GoldCount <= 0)
        {
            GameOver(DeathMessage.NoGold);
        }

        if(debugMode && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void GameStarted()
    {
        _GameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        Invoke("InitPlatformSpawner", 3f);
    }

    private void InitPlatformSpawner()
    {
        blockSpawner.SpawnPlatform();
    }

    public void GameOver(DeathMessage deathMessage)
    {
        if (debugMode) return;
        _textGameOverCondition.text = StringManager.Instance.GetDeathMessage(deathMessage);
        isGameOver = true;
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton);
        Time.timeScale = 0;
    }
}
