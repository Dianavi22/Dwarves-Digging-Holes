using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public PlatformSpawner platformSpawner;

    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] private GameObject _PauseCanvas;
    public bool isPaused = false;

    [SerializeField] private EventSystem _eventSystem;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Pause(FindFirstObjectByType<PlayerActions>().gameObject);
        }
    }
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
        GameStarted();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(FindFirstObjectByType<PlayerActions>().gameObject);
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
        platformSpawner.SpawnPlatform();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _GameOverCanvas.SetActive(true);

    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        print("Main Menu");
    }

    public void Pause(GameObject _currentPlayer)
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            _PauseCanvas.SetActive(true);
            _currentPlayer.GetComponent<PlayerMovements>().enabled = false;
            _eventSystem.firstSelectedGameObject = _currentPlayer;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            _PauseCanvas.SetActive(false);
            _currentPlayer.GetComponent<PlayerMovements>().enabled = true;

            _eventSystem.firstSelectedGameObject = null;

            isPaused = false;


        }
    }

}
