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
    public void StartGame()
    {
        SceneManager.LoadScene(1);
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

    }

    public void LoadSettingScene()
    {
        _settingsWindow.SetActive(true);
        _eventSystem.firstSelectedGameObject = _closeButtonSettings;
    }
}
