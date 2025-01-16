using FMODUnity;
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

    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _rebindJump;

    [SerializeField] private GameObject _inputCanvas;

    [SerializeField] private EventReference[] _stateMenuEvent;
    [SerializeField] GameObject _circleTransition;

    private bool _scaleButton;
    [SerializeField] List<GameObject> _button;

    public static UIPauseManager Instance; // A static reference to the GameManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void RetryGame()
    {
        StartCoroutine(FadeTransition(true));
    }

    private IEnumerator FadeTransition(bool isRetry)
    {
        _circleTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        if (isRetry)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void MainMenu()
    {
        StartCoroutine(FadeTransition(false));
    }

    public void Pause()
    {
        if (!GameManager.Instance.isGameOver)
        {
            isPaused = !isPaused;

            Utils.Sound.SetGlobalVolumeExcept(isPaused ? 0.75f : 1f, RuntimeManager.GetBus("bus:/UI"));
            RuntimeManager.StudioSystem.setParameterByName("LowPassMenu", isPaused ? 1 : 0);

            RuntimeManager.PlayOneShot(_stateMenuEvent[isPaused ? 1 : 0]);

            Time.timeScale = isPaused ? 0 : 1;
            _PauseCanvas.SetActive(isPaused);
            _inputCanvas.SetActive(!isPaused);

            if (isPaused)
            {
                EventSystem.current.SetSelectedGameObject(_resumeButton);
            }
        }
    }


    public void OpenInputCanvas()
    {
        _scaleButton = true;
        _inputCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_rebindJump);
    }

    public void CloseInputMenu()
    {
        _inputCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_resumeButton);
    }

    void OnDestroy()
    {
        RuntimeManager.StudioSystem.setParameterByName("LowPassMenu", 0);
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
        Vector3 targetScale = new Vector3(3.577049f, 3.577049f, 3.577049f);

        for (int i = 0; i < _button.Count; i++)
        {
            if (_button[i] != null)
            {
                _button[i].transform.localScale = targetScale;
            }
        }

        _scaleButton = false;
    }
}
