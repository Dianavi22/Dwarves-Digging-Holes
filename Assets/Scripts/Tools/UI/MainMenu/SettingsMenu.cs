using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    Resolution[] resolutions;
    [SerializeField] TMP_Text resolutionDropDown;
    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject _settingsButtonMM;

    [SerializeField] private CanvasGroup _buttonsCanvaGroup;
    [SerializeField] private MainMenuManager _mmm;

    [SerializeField] Button changeResolutionButton;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == 1920 && resolutions[i].height == 1080)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, Screen.fullScreen);

        if (changeResolutionButton != null)
        {
            changeResolutionButton.onClick.AddListener(ChangeResolution);
        }
        UpdateResolutionText();
    }

    public void ChangeResolution()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutions.Length;
        Resolution newResolution = resolutions[currentResolutionIndex];

        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        if (resolutionDropDown != null)
        {
            resolutionDropDown.text = $"{resolutions[currentResolutionIndex].width} x {resolutions[currentResolutionIndex].height}";
        }
    }

    public void SetFullScreen(bool isFullScreen) // Mettre ou enlever le fullScreen
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetMusic(float volume) // Changer le volume de la musique
    {
        audioMixer.SetFloat("music", volume);
    }
    public void SetSound(float volume) // Changer le volume du son
    {
        audioMixer.SetFloat("sound", volume);
    }

    public void CloseSettings()
    {
        if (GameManager.Instance.isInMainMenu)
        {
            _mmm.ShowButtons();
            _buttonsCanvaGroup.interactable = true;
        }
        EventSystem.current.SetSelectedGameObject(_settingsButtonMM);
        SettingsWindow.SetActive(false);
    }
}
