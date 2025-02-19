using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
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
    [SerializeField] private GameObject levelCompleteCanvaUI;   
    [SerializeField] GameObject _retryButton;
    [SerializeField] TMP_Text _textGameOverCondition;

    [SerializeField] GameObject _skipTuto;
    [SerializeField] GameObject _scoreText;
    [SerializeField] GameObject _exitText;
    [SerializeField] GameObject _circleTransition;

    [SerializeField] Button _backButton;
    [SerializeField] Button _backbuttonLevelComplete;
    [SerializeField] GameObject _gameOverCanva;

    [SerializeField] List<GameObject> _playerStats;
    [SerializeField] List<GameObject> _buttons;

    [Header("Global Sound")]
    [SerializeField] EventReference submitEvent;
    [SerializeField] EventReference navigateEvent;


    [Header("Other")]
    [SerializeField] private PlatformSpawner blockSpawner;
    [SerializeField] ParticleSystem _gameOverPart;
    [SerializeField] IntroGame _introGame;
    [SerializeField] GameObject _cam;

    [SerializeField] private EventReference piercingTonesSound;
    [SerializeField] private int repetitions = 3;
    [SerializeField] private float interval = 0.5f;
    [SerializeField] private EventReference metalExplosionSound;
    private bool isMetalExplosionEnd = false;

    private Score _score;
    private GoldChariot _goldChariot;
    private Tuto _tuto;
    [SerializeField] GameObject _PickaxeUI;
    public TMP_Text nbPickaxeUI;
    [SerializeField] TMP_Text _nbMaxPickaxeUI;
    [SerializeField] private EventReference showPanelTutoSound;

    public bool isEnding = false;
    private bool isCoroutineRunning = false;
    private bool _scaleButton = false;
    private bool _isTutoActive = true;
    public bool isChariotWin = false;
    public static GameManager Instance; // A static reference to the GameManager instance
    private bool _isGamewin = false;

    public EventReference GetSubmitUISound() => submitEvent;
    public EventReference GetNavigateUISound() => navigateEvent;

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
        Cursor.visible = false;
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
        ShowPanelTutoSound();
    }

    public IEnumerator StartGame()
    {
        GameOST.Instance.StartMusic();
        isGameStarted = true;
        _scoreText.SetActive(true);
        _exitText.SetActive(true);
        Invoke("OutExitPanel", 10);
        _score.isStartScore = true;
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(3f, 0.2f);
        blockSpawner.SpawnPlatform();
        CurrentScrollingSpeed = this.Difficulty.ScrollingSpeed;
        yield return new WaitForSeconds(70); //70
        EventManager.Instance.LaunchEvent();
    }

    private void OutExitPanel()
    {
        _exitText.GetComponent<Animator>().SetTrigger("OutPanel");
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
        levelCompleteCanvaUI.SetActive(false);
        StartCoroutine(ShowStats());
        _scaleButton = true;

    }

    private void Update()
    {
        if (_scaleButton)
        {
            UpdateButtonScale();
        }
        if (!isInMainMenu)
        {
            nbPickaxeUI.text = NbPickaxe.ToString();
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

        if(_isGamewin) {
            levelCompleteCanvaUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(levelCompleteCanvaUI.transform.GetChild(4).gameObject);
        }
        else {
            _gameOverCanva.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_retryButton);
        }

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

    public void SetStatsParent() {
        _isGamewin = true;
        GameObject stats = StatsManager.Instance.GetStatsGameObject();
        stats.transform.SetParent(levelCompleteCanvas.transform);
        _backButton.transform.SetParent(levelCompleteCanvas.transform);
    }
    public IEnumerator LevelComplete()
    {
        if (isGameOver) yield break;

        _gameOverCanva = levelCompleteCanvaUI;
        _backButton = _backbuttonLevelComplete;
        _retryButton = levelCompleteCanvaUI.transform.GetChild(2).gameObject;
        SetStatsParent();

        //_textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
        //_gameOverPart.gameObject.SetActive(true);
        isGameOver = true;
        _goldChariot.StopParticle();
        EventManager.Instance.enabled = false;
        yield return new WaitForSeconds(2f);
        bool newBest = _score.CheckBestScore();
        levelCompleteCanvas.SetActive(true);
        RuntimeManager.StudioSystem.setParameterByName("LowPassMenu", 1);
        EventSystem.current.SetSelectedGameObject(_retryButton);
        yield return null;
    }

    public IEnumerator GameOver(Message deathMessage, bool isGoldChariotDestroyed)
    {
        if (_tuto.isInTuto)
        {
            print("Skip tuto");
            SkipTuto();
        }
        
        if (isGameOver || isEnding) yield break;

        StatsManager.Instance.EndGame();
        yield return new WaitForSeconds(1f);
        _textGameOverCondition.text = StringManager.Instance.GetSentence(deathMessage);
        _gameOverPart.GetComponent<ParticleSystem>().Play();
        PiercingTonesSound();
        isGameOver = true;
        _goldChariot.StopParticle();
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(5.5f, 0.2f);
        EventManager.Instance.enabled = false;
        yield return new WaitForSeconds(3.8f);
        _goldChariot.HideGfx(isGoldChariotDestroyed);
        MetalExplosionSound();
        yield return new WaitForSeconds(2f);
        _GameOverCanvas.SetActive(true);
        RuntimeManager.StudioSystem.setParameterByName("LowPassMenu", 1);
        // ? Activer un message / effet si record battu
        bool newBest = _score.CheckBestScore();
        CurrentScrollingSpeed = 0f;
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }


    #region Sounds
    private void ShowPanelTutoSound()
    {
        RuntimeManager.PlayOneShot(showPanelTutoSound, transform.position);
    }
    private void PiercingTonesSound()
    {
        StartCoroutine(PlaySoundWithDelay());
    }
    private IEnumerator PlaySoundWithDelay()
    {
        for (int i = 0; i < repetitions; i++)
        {
            RuntimeManager.PlayOneShot(piercingTonesSound, transform.position);
            yield return new WaitForSeconds(interval);
        }
    }
    private void MetalExplosionSound()
    {
        if (!isMetalExplosionEnd){
            isMetalExplosionEnd = true;
            RuntimeManager.PlayOneShot(metalExplosionSound, transform.position);
        }
    }


    #endregion

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        RuntimeManager.StudioSystem.setParameterByName("LowPassMenu", 0);
    }
}
