using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    [Header("DebugMode")]
    public bool debugMode;
    public bool isDisableEventManager;

    [SerializeField] private PlatformSpawner blockSpawner;

    [SerializeField] private PhysicMaterial holderPhysicMaterial;

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
    [SerializeField] IntroGame _introGame;
    [SerializeField] Lava _lava;
    public bool isGameOver = false;
    public bool isInMainMenu = false;
    [SerializeField] EventManager _eventManager;
    private GoldChariot _goldChariot;
    [SerializeField] private Tuto _tuto;
        
    private float _baseSpeed;
    public static GameManager Instance; // A static reference to the GameManager instance
    public bool passTuto = false;

    [SerializeField] GameObject _skipTuto;
    [SerializeField] GameObject _scoreText;
    [SerializeField] GameObject _circleTransition;
    [SerializeField] Score _score;


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

        if (!isInMainMenu)
        {
            _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
            _goldChariot.GoldCount = Difficulty.NbStartingGold;
        }

        foreach (Player p in GamePadsController.Instance.PlayerList)
        {
            p.GetMovement().SetStats(Difficulty.PlayerStats);
            p.GetFatigue().DefineStats(Difficulty.MiningFatigue, Difficulty.PushCartFatigue);
        }

        foreach (Pickaxe pickaxe in FindObjectsOfType<Pickaxe>())
            AddPickaxe(pickaxe);
        if (!isInMainMenu) GameStarted();
        _circleTransition.SetActive(true);

    }


    private IEnumerator StartParty()
    {
            _baseSpeed = this.Difficulty.ScrollingSpeed;
        if (!isInMainMenu)
        {
            _eventManager.scrollSpeed = _baseSpeed;
            this.Difficulty.ScrollingSpeed = 0;
        }
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(_introGame.LadderIntro());
        yield return new WaitForSeconds(2);
        if (passTuto && !_tuto.isInTuto)
        {
            SkipTuto();
        }
        else
        {
            _skipTuto.SetActive(true);
            _tuto.startTuto = true;
        }

    }

    public void SkipTuto()
    {
        StartCoroutine(_lava.CooldownLava());
        _skipTuto.SetActive(false);
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        _scoreText.SetActive(true);
        _score.isStartScore = true;
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(3f, 0.2f);
        Invoke(nameof(InitPlatformSpawner), 3f);
        this.Difficulty.ScrollingSpeed = _baseSpeed;
        yield return new WaitForSeconds(70);
        _eventManager.LaunchEvent();
    }

    void Update()
    {
        if (!isInMainMenu && _goldChariot.GoldCount <= 0 && !isGameOver && !debugMode)
        {
            StartCoroutine(GameOver(Message.NoGold));
        }
    }

    private void GameStarted()
    {
        _GameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        StartCoroutine(StartParty());
    }

    private void InitPlatformSpawner()
    {
        blockSpawner.SpawnPlatform();
    }

    public IEnumerator GameOver(Message deathMessage)
    {
        if (!isGameOver)
        {
            StatsManager.Instance.EndGame();

            _textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
            _gameOverPart.gameObject.SetActive(true);
            isGameOver = true;
            _goldChariot.HideChariotText();
            TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(5.5f, 0.2f);
            _eventManager.enabled = false;
            yield return new WaitForSeconds(3.5f);
            _goldChariot.HideGfx();
            yield return new WaitForSeconds(2f);
            _GameOverCanvas.SetActive(true);
            // ? Activer un message / effet si record battu
            bool newBest = TargetManager.Instance.GetGameObject<Score>(Target.Score).CheckBestScore();
            this.Difficulty.ScrollingSpeed = _baseSpeed;
            EventSystem.current.SetSelectedGameObject(_retryButton);
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        this.Difficulty.ScrollingSpeed = _baseSpeed;
        holderPhysicMaterial.dynamicFriction = 1;
        holderPhysicMaterial.staticFriction = 1;
    }
}
