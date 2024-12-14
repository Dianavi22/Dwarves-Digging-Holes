using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _closeButtonSettings;
    [SerializeField] GameObject _circleTransition;
    [SerializeField] GameObject _circleTransitionIn;
    [SerializeField] GameObject _buttons;
    [SerializeField] GameObject _title;
    [SerializeField] GameObject _credits;

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
        Invoke("ActiveButtons", 20);
    }

    private void ActiveButtons()
    {
        _buttons.SetActive(true);
        _title.SetActive(true);
        _credits.SetActive(false);
    }

    public void LoadSettingScene()
    {
        _settingsWindow.SetActive(true);
        _eventSystem.firstSelectedGameObject = _closeButtonSettings;
    }

    private IEnumerator CircleTransition()
    {
        _circleTransition.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
