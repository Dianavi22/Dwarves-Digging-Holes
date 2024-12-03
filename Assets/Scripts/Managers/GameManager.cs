using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public bool debugMode;
    [SerializeField] private PlatformSpawner blockSpawner;
    [SerializeField] Platform _platform;

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
    #endregion

    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;

    [SerializeField] TMP_Text _textGameOverCondition;

    [SerializeField] ParticleSystem _gameOverPart;

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
    }

    void Start()
    {
        if (debugMode) Debug.LogWarning("GAME MANAGER DEBUG MODE");

        // Select the difficulty
        Difficulty = m_DifficultyList[GamePadsController.Instance.PlayerList.Count <= 2 ? 0 : 1];
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
        _goldChariot.GoldCount = Difficulty.NbStartingGold;

        foreach (Pickaxe pickaxe in FindObjectsOfType<Pickaxe>())
            AddPickaxe(pickaxe);
        GameStarted();
    }

    void Update()
    {
        if (_goldChariot.GoldCount <= 0 && !isGameOver)
        {
            if (!debugMode)
            {
                StartCoroutine(GameOver(DeathMessage.NoGold));
            }
        }

        if(debugMode && Input.GetKeyDown(KeyCode.R))
        {
           StartCoroutine(GameOver(DeathMessage.NoGold));
            
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

    public IEnumerator GameOver(DeathMessage deathMessage)
    {
        _textGameOverCondition.text = StringManager.Instance.GetDeathMessage(deathMessage);
        _gameOverPart.gameObject.SetActive(true);
        isGameOver = true;
        _goldChariot.HideChariotText();
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(5.5f,0.2f);
        yield return new WaitForSeconds(3.5f);
        _goldChariot.HideGfx();
        yield return new WaitForSeconds(2f);
        _GameOverCanvas.SetActive(true);
        // ? Activer un message / effet si record battu
        bool newBest = TargetManager.Instance.GetGameObject<Score>(Target.Score).CheckBestScore();
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }
}
