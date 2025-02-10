using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Linq;

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

    [SerializeField] private GameObject levelCompleteCanvas;   
    [SerializeField] GameObject _retryButton;
    [SerializeField] TMP_Text _textGameOverCondition;

    [SerializeField] GameObject _skipTuto;
    [SerializeField] GameObject _scoreText;
    [SerializeField] GameObject _circleTransition;

    [SerializeField] Button _backButton;
    [SerializeField] GameObject _gameOverCanva;

    [SerializeField] List<GameObject> _playerStats;
    [SerializeField] List<GameObject> _buttons;

    [Header("Other")]
    [SerializeField] private PlatformSpawner blockSpawner;
    [SerializeField] ParticleSystem _gameOverPart;
    [SerializeField] IntroGame _introGame;

    private Score _score;
    private GoldChariot _goldChariot;
    private Tuto _tuto;
    [SerializeField] GameObject _PickaxeUI;
    public TMP_Text nbPickaxeUI;
    [SerializeField] TMP_Text _nbMaxPickaxeUI;

    
    private bool isCoroutineRunning = false;
    private bool _scaleButton = false;
    private bool _isTutoActive = true;
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
        _isTutoActive = PlayerPrefs.GetInt("TutoActive") == 1;
        if (debugMode) Debug.LogWarning("GAME MANAGER DEBUG MODE");

        // Select the difficulty
        Difficulty = isInMainMenu ? m_DifficultyList[0] :  m_DifficultyList.First(x => x.DifficultyName == PlayerPrefs.GetString(Utils.Constant.DIFFICULTY_KEY));
        //Difficulty = m_DifficultyList[GamePadsController.Instance.PlayerList.Count <= 2 ? 0 : 1];

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
            _goldChariot._currentGoldCount = Difficulty.NbStartingGold;
            _score = TargetManager.Instance.GetGameObject<Score>();
            _tuto = TargetManager.Instance.GetGameObject<Tuto>();

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
        if (!_isTutoActive)
        {
            passTuto=true; 
        }
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
        _nbMaxPickaxeUI.text = MaxNbPickaxe.ToString();
    }

    public void SkipTuto()
    {
        StartCoroutine(TargetManager.Instance.GetGameObject<Lava>().CooldownLava());
        _skipTuto.SetActive(false);
        _tuto.isInTuto = false;
        _PickaxeUI.SetActive(true);
    }

    public IEnumerator StartGame()
    {
        isGameStarted = true;
        _scoreText.SetActive(true);
        _score.isStartScore = true;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(3f, 0.2f);
        blockSpawner.SpawnPlatform();
        CurrentScrollingSpeed = this.Difficulty.ScrollingSpeed;
        yield return new WaitForSeconds(70); //70
        EventManager.Instance.LaunchEvent();
    }

    private void GameStarted()
    {
        _GameOverCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        StartCoroutine(StartParty());
    }

    #region endgame stats
    public void ShowCardsFunc()
    {
        _gameOverCanva.SetActive(false);
        StartCoroutine(ShowStats());
        _scaleButton = true;

    }

    private void Update()
    {
        if (_scaleButton)
        {
            UpdateButtonScale();
        }
    }

    private void UpdateButtonScale()
    {
        Vector3 targetScale = new Vector3(2.1385f, 2.1385f, 2.1385f);

        for (int i = 0; i < _buttons.Count; i++)
        {
            if (_buttons[i] != null)
            {
                _buttons[i].transform.localScale = targetScale;
            }
        }

        _scaleButton = false;
    }

    public void HideCards()
    {
        if (isCoroutineRunning)
        {
            _playerStats.ForEach(stat => stat.SetActive(true));
            isCoroutineRunning = false;
            return;
        }
        _backButton.gameObject.SetActive(false);

        _playerStats.ForEach(stat => stat.SetActive(false));

        _gameOverCanva.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton);

    }

    private IEnumerator ShowStats()
    {
        isCoroutineRunning = true;
        _backButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_backButton.gameObject);
        for (int i = 0; i < _playerStats.Count; i++)
        {
            _playerStats[i].SetActive(true);
            yield return new WaitForSecondsRealtime(0.35f);
        }
        isCoroutineRunning = false;
    }
    #endregion

    public IEnumerator LevelComplete()
    {
        if (isGameOver) yield break;

        StatsManager.Instance.EndGame();

        //_textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
        //_gameOverPart.gameObject.SetActive(true);
        isGameOver = true;
        _goldChariot.StopParticle();
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(5.5f, 0.2f);
        EventManager.Instance.enabled = false;
        yield return new WaitForSeconds(2f);
        bool newBest = _score.CheckBestScore();
        levelCompleteCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(levelCompleteCanvas.transform.GetChild(4).gameObject);
        yield return null;
    }

    public IEnumerator GameOver(Message deathMessage, bool isGoldChariotDestroyed)
    {
        if (_tuto.isInTuto)
        {
            print("Skip tuto");
            SkipTuto();
        }
        
        if (isGameOver) yield break;

        StatsManager.Instance.EndGame();

        _textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
        _gameOverPart.gameObject.SetActive(true);
        isGameOver = true;
        _goldChariot.StopParticle();
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(5.5f, 0.2f);
        EventManager.Instance.enabled = false;
        yield return new WaitForSeconds(3.5f);
        _goldChariot.HideGfx(isGoldChariotDestroyed);
        yield return new WaitForSeconds(2f);
        _GameOverCanvas.SetActive(true);
        // ? Activer un message / effet si record battu
        bool newBest = _score.CheckBestScore();
        CurrentScrollingSpeed = 0f;
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }
}
