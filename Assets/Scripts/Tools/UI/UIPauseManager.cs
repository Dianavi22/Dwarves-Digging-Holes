using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPauseManager : MonoBehaviour
{

    [SerializeField] private GameObject _PauseCanvas;
    [HideInInspector] public bool isPaused = false;

    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _rebindJump;
    [SerializeField] private GameObject _backbuttonSettings;

    [SerializeField] private GameObject _inputCanvas;
    [SerializeField] private GameObject _settingsCanvas;

    [SerializeField] private EventReference[] _stateMenuEvent;
    [SerializeField] GameObject _circleTransition;
    [SerializeField] GameObject _controlsPanel;
    [SerializeField] Button _controlsPanelButton;

    public bool scaleButton;
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

    public void OpenControlPanel()
    {
        if (_controlsPanel.activeInHierarchy)
        {
            _controlsPanel.SetActive(false);
            for (int i = 0; i < _button.Count; i++)
            {
                if (_button[i] != null)
                {
                    _button[i].SetActive(true);
                }
            }
        }
        else {
            _controlsPanel.SetActive(true);
            for (int i = 0; i < _button.Count; i++)
            {
                if (_button[i] != null)
                {
                    _button[i].SetActive(false);
                }
            }
        }
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

            if (!isPaused)
            {
                _controlsPanel.SetActive(false);
                _settingsCanvas.SetActive(false);
                for (int i = 0; i < _button.Count; i++)
                {
                    if (_button[i] != null)
                    {
                        _button[i].SetActive(true);
                    }
                }
            }

            if (isPaused)
            {
                EventSystem.current.SetSelectedGameObject(_resumeButton);
            }
          
        }
    }


    public void OpenInputCanvas()
    {
        scaleButton = true;
        _inputCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_rebindJump);
    }
    public void OpenSettingsMenu()
    {
        _backbuttonSettings.transform.localScale = new Vector3(0.212207913f, 0.427238166f, 0.212207913f);
        scaleButton = true;
        _settingsCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_backbuttonSettings);
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
        if (scaleButton)
        {
            UpdateButtonScale();
        }

    }

    private void UpdateButtonScale()
    {
        Vector3 targetScale = new Vector3(2.4582f, 2.4582f, 2.4582f);

        for (int i = 0; i < _button.Count; i++)
        {
            if (_button[i] != null)
            {
                _button[i].transform.localScale = targetScale;
            }
        }

        scaleButton = false;
    }
}
