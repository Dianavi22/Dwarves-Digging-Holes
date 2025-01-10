using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("DebugMode")]
    public bool debugMode;
    public bool isDisableEventManager;

    public bool isGameStarted = false;
    public bool isGameOver = false;
    public bool isInMainMenu = false;
    public bool passTuto = false;

    #region Difficulty
    [Header("Difficulty")]
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

    public float CurrentScrollingSpeed { private set; get; }

    public void SetScrollingSpeed(float value)
    {
        CurrentScrollingSpeed = value;
    }
    #endregion

    [Header("Canvas")]
    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] GameObject _retryButton;
    [SerializeField] TMP_Text _textGameOverCondition;

    [SerializeField] GameObject _skipTuto;
    [SerializeField] GameObject _scoreText;
    [SerializeField] GameObject _circleTransition;

    [SerializeField] Button _backButton;
    [SerializeField] GameObject _gameOverCanva;

    [SerializeField] List<GameObject> _playerStats;

    [Header("Other")]
    [SerializeField] private PlatformSpawner blockSpawner;
    [SerializeField] ParticleSystem _gameOverPart;
    [SerializeField] IntroGame _introGame;

    [SerializeField] LevelCompleteManager _levelCompleteManager;

    private Score _score;
    private GoldChariot _goldChariot;
    private Tuto _tuto;
    private EventManager _eventManager;

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

        foreach (Player p in GamePadsController.Instance.PlayerList)
        {
            p.GetMovement().SetStats(Difficulty.PlayerStats);
            p.GetFatigue().DefineStats(Difficulty.MiningFatigue, Difficulty.PushCartFatigue);
        }

        foreach (Pickaxe pickaxe in FindObjectsOfType<Pickaxe>())
            AddPickaxe(pickaxe);


        if (!isInMainMenu)
        {
            _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
            _goldChariot.GoldCount = Difficulty.NbStartingGold;
            _score = TargetManager.Instance.GetGameObject<Score>();
            _tuto = TargetManager.Instance.GetGameObject<Tuto>();
            _eventManager = EventManager.Instance;

            GameStarted();
        }

        _circleTransition.SetActive(true);
    }

    private IEnumerator StartParty()
    {
        CurrentScrollingSpeed = 0;
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(_introGame.LadderIntro());
        yield return new WaitForSeconds(2);
        if (passTuto && !_tuto.isInTuto)
        {
            SkipTuto();
            StartCoroutine(StartGame());
        }
        else
        {
            _skipTuto.SetActive(true);
            _tuto.isInTuto = true;
            _tuto.startTuto = true;
        }

    }

    public void SkipTuto()
    {
        StartCoroutine(TargetManager.Instance.GetGameObject<Lava>().CooldownLava());
        _skipTuto.SetActive(false);
        _tuto.isInTuto = false;
    }

    public IEnumerator StartGame()
    {
        isGameStarted = true;
        _levelCompleteManager.StartGame();
        _scoreText.SetActive(true);
        _score.isStartScore = true;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(3f, 0.2f);
        blockSpawner.SpawnPlatform();
        CurrentScrollingSpeed = this.Difficulty.ScrollingSpeed;
        yield return new WaitForSeconds(70);
        _eventManager.LaunchEvent();
    }

    private void GameStarted()
    {
        _GameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        StartCoroutine(StartParty());
    }

    public IEnumerator GameOver(Message deathMessage)
    {
        if (isGameOver) yield break;

        StatsManager.Instance.EndGame();

        _textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
        _gameOverPart.gameObject.SetActive(true);
        isGameOver = true;
        _goldChariot.HideChariotText();
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(5.5f, 0.2f);
        _eventManager.enabled = false;
        yield return new WaitForSeconds(3.5f);
        _goldChariot.HideGfx();
        yield return new WaitForSeconds(2f);
        _GameOverCanvas.SetActive(true);
        // ? Activer un message / effet si record battu
        bool newBest = _score.CheckBestScore();
        CurrentScrollingSpeed = 0f;
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }

    public void ShowCardsFunc()
    {
        _gameOverCanva.SetActive(false);
        StartCoroutine(ShowStats());
    }

    public void HideCards()
    {
        _gameOverCanva.SetActive(true);
        _backButton.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_retryButton);
        for (int i = 0; i < _playerStats.Count; i++)
        {
            _playerStats[i].SetActive(false);
        }
    }

    private IEnumerator ShowStats()
    {
        _backButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_backButton.gameObject);
        for (int i = 0; i < _playerStats.Count; i++)
        {
            _playerStats[i].SetActive(true);
            yield return new WaitForSecondsRealtime(0.35f);
        }
    }
    
    public IEnumerator LevelComplete(LevelCompleteManager levelCompleteManager)
    {
        if (isGameOver) yield break;

        Debug.Log("LevelComplete");
        isGameOver = true;
        _goldChariot.HideChariotText();
        _goldChariot.HideGfx();
        _eventManager.enabled = false;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(5.5f, 0.2f);
        levelCompleteManager.blockSpawner.SetActive(false);
        levelCompleteManager.lavaGFX.SetActive(false);
        yield return new WaitForSeconds(5.5f);
        levelCompleteManager.canvas.SetActive(true);
        CurrentScrollingSpeed = 0f;
        EventSystem.current.SetSelectedGameObject(levelCompleteManager.mainMenuButton);
        yield return null;
    }
}
