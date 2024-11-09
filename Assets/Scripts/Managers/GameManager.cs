using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public bool debugMode;
    [SerializeField] private PlatformSpawner blockSpawner;

    #region Difficulty
    // The difficulty have to be listed from the easiest to the hardest
    [SerializeField] private List<Difficulty> m_DifficultyList;
    public Difficulty Difficulty { private set; get; }

    #region Nb Pickaxe Handler
    public int MaxNbPickaxe
    {
        get { return Math.Min(Difficulty.MaxNumberPickaxe, GamePadsController.Instance.PlayerList.Count); }
    }
    private int _nbPickaxe;
    public int NbPickaxe
    {
        get => _nbPickaxe;
        set
        {
            if (!CanCreatePickaxe) return;
            _nbPickaxe = value;
        }
    }
    public bool CanCreatePickaxe => _nbPickaxe < MaxNbPickaxe;
    public void AddPickaxe(Pickaxe p)
    {
        if (!CanCreatePickaxe) Destroy(p.gameObject);
        p.HealthPoint = Difficulty.PickaxeDurability;
        NbPickaxe++;
    }
    #endregion

    public float ScrollingSpeed
    {
        // Is used in Platform script
        get { return Difficulty.ScrollingSpeed; }
    }
    #endregion

    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;

    [SerializeField] TMP_Text _textGameOverCondition;

    public bool isGameOver = false;

    private GoldChariot _goldChariot;

    public static GameManager Instance; // A static reference to the GameManager instance
    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        foreach (Pickaxe pickaxe in FindObjectsOfType<Pickaxe>()) 
            AddPickaxe(pickaxe);

        // Select the difficulty
    }

    void Start()
    {
        if (debugMode) Debug.LogWarning("GAME MANAGER DEBUG MODE");

        Difficulty = m_DifficultyList[GamePadsController.Instance.PlayerList.Count <= 2 ? 0 : 1];
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
