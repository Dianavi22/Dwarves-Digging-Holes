using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    Resolution[] resolutions;
    [SerializeField] TMP_Dropdown resolutionDropDown;
    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject _settingsButtonMM;

    [SerializeField] private CanvasGroup _buttonsCanvaGroup;
    [SerializeField] private MainMenuManager _mmm;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex) // Changer la resolution de l'ecran
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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
        _mmm.ShowButtons();
        _buttonsCanvaGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(_settingsButtonMM);
        SettingsWindow.SetActive(false);
    }
}
