using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIPauseManager : MonoBehaviour
{

    [SerializeField] private GameObject _PauseCanvas;
    [HideInInspector] public bool isPaused = false;

    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _retryButton;
    [SerializeField] private GameObject _rebindJump;

    [SerializeField] private GameObject _inputCanvas;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Pause(FindFirstObjectByType<PlayerActions>().gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(FindFirstObjectByType<PlayerActions>().gameObject);
        }
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
            EventSystem.current.SetSelectedGameObject(_retryButton);
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            _inputCanvas.SetActive(false);
            _PauseCanvas.SetActive(false);
            _currentPlayer.GetComponent<PlayerMovements>().enabled = true;


            isPaused = false;


        }
    }

    public void OpenInputCanvas()
    {
        _inputCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_rebindJump);
    }
}
