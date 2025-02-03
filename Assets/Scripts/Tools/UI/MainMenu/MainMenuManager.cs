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
    [SerializeField] Color _bc;
    [SerializeField] private CanvasGroup _buttonsCanvaGroup;
    private bool _scaleButton = false;

    [SerializeField] GameObject _canvasSettings;
    [SerializeField] GameObject _pivotStartSettings;
    [SerializeField] GameObject _buttonSettingsStart;

    [SerializeField] GameObject _backButton;
    [SerializeField] GameObject _bbSettings;
    [SerializeField] Toggle _toggle;
    private void Start()
    {
        StartCoroutine(StartCanvas());
        _toggle.isOn = PlayerPrefs.GetInt("TutoActive") == 1;
    }

    private IEnumerator StartCanvas()
    {
        Time.timeScale = 1;
        _circleTransitionIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);

    }
    public void StartGame()
    {
        _scaleButton = true;
        _pivotStartSettings.SetActive(true);
        _buttons.SetActive(false);
        _title.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_buttonSettingsStart);
    }

    

    public void StartParty()
    {
        IsTutoActive();
        StartCoroutine(CircleTransition());
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public bool IsTutoActive()
    {
        int i;
        if (_toggle.isOn)
        {
            i = 1;
        }
        else
        {
            i = 0;

        }
        if (!PlayerPrefs.HasKey("TutoActive"))
        {
            PlayerPrefs.SetInt("TutoActive", i); 
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("TutoActive", i);

        }
     
        return PlayerPrefs.GetInt("TutoActive") == i;
    }

    public void LoadCreditScene()
    {
        _scaleButton = true;
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
        _scaleButton = true;
        _buttons.SetActive(true);
        _title.SetActive(true);

    }

    public void LoadSettingScene()
    {
        _bbSettings.transform.localScale = new Vector3(0.212207913f, 0.427238166f, 0.212207913f);
        _scaleButton = true;
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
        SceneManager.LoadScene(2);
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
        Vector3 targetScale = new Vector3(3.6f, 3.6f, 3.6f);

        for (int i = 0; i < _button.Count; i++)
        {
            if (_button[i] != null)
            {
                _button[i].GetComponentInChildren<TMP_Text>().color = _bc;
                _button[i].transform.localScale = targetScale;
            }
        }
        _backButton.transform.localScale = new Vector3(1,1,1);
        _scaleButton = false;
        _startButton.GetComponentInChildren<TMP_Text>().color = Color.white;
    }
}
