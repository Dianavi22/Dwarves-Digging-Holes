using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private GameObject _closeButtonSettings;
    [SerializeField] GameObject _circleTransition;
    [SerializeField] GameObject _circleTransitionIn;
    [SerializeField] GameObject _buttons;
    [SerializeField] GameObject _title;
    [SerializeField] GameObject _credits;
    [SerializeField] GameObject _stopCredits;
    [SerializeField] GameObject _startButton;
    [SerializeField] private CanvasGroup _buttonsCanvaGroup;

    private void Start()
    {
        StartCoroutine(StartCanvas());
    }

    private IEnumerator StartCanvas()
    {
        Time.timeScale = 1;
        _circleTransitionIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);
      
    }
    public void StartGame()
    {
        StartCoroutine(CircleTransition());
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void LoadCreditScene()
    {
        _buttons.SetActive(false);
        _title.SetActive(false);
        _credits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_stopCredits);
        Invoke("ActiveButtons", 20);
    }

    public void ActiveButtons()
    {
        ShowButtons();
        _credits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_startButton);

    }

    public void ShowButtons()
    {
        _buttons.SetActive(true);
        _title.SetActive(true);
    }

    public void LoadSettingScene()
    {
        _title.gameObject.SetActive(false);
        _buttons.gameObject.SetActive(false);
        _buttonsCanvaGroup.interactable = false;
        _settingsWindow.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_closeButtonSettings);
    }

    private IEnumerator CircleTransition()
    {
        _circleTransition.SetActive(true);
        yield return new WaitForSeconds(1.7f);
        SceneManager.LoadScene(1);
    }
}
