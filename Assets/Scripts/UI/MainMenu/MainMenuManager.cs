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
    [SerializeField] List<GameObject> _button;
    [SerializeField] GameObject _title;
    [SerializeField] GameObject _credits;
    [SerializeField] GameObject _stopCredits;
    [SerializeField] GameObject _startButton;
    [SerializeField] private CanvasGroup _buttonsCanvaGroup;
    private bool _scaleButton = false;

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
        _scaleButton = true;
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

    private void Update()
    {
        if (_scaleButton)
        {
            UpdateButtonScale();


        }
    }

    private void UpdateButtonScale()
    {
        Vector3 scale = _button[0].GetComponent<Transform>().localScale;
        scale.x = 3.577049f;
        scale.y = 3.577049f;
        scale.z = 3.577049f;
        _button[0].GetComponent<Transform>().localScale = scale;

        Vector3 scale2 = _button[1].GetComponent<Transform>().localScale;
        scale2.x = 3.577049f;
        scale2.y = 3.577049f;
        scale2.z = 3.577049f;
        _button[1].GetComponent<Transform>().localScale = scale2;

        Vector3 scale3 = _button[2].GetComponent<Transform>().localScale;
        scale3.x = 3.577049f;
        scale3.y = 3.577049f;
        scale3.z = 3.577049f;

        _button[2].GetComponent<Transform>().localScale = scale3;

        Vector3 scale4 = _button[3].GetComponent<Transform>().localScale;
        scale4.x = 3.577049f;
        scale4.y = 3.577049f;
        scale4.z = 3.577049f;

        _button[3].GetComponent<Transform>().localScale = scale3;
        _scaleButton = false;

    }
}
