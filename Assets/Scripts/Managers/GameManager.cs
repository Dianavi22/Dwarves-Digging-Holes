using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlatformSpawner platformSpawner;

    public static GameManager Instance; // A static reference to the GameManager instance
    [SerializeField] private GameObject _GameOverCanvas;
    [SerializeField] private GameObject _PauseCanvas;
    private bool _isPaused = false;


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Pause();
        }
    }
    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
    }



    void Start()
    {
        GameStarted();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
                Pause();
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

    public void Pause()
    {
        if (!_isPaused)
        {
            Time.timeScale = 0;
            _PauseCanvas.SetActive(true);
            _isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            _PauseCanvas.SetActive(false);
            _isPaused = false;


        }
    }

}
