using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using FMODUnity;

public class GameSettingsSelection : MonoBehaviour
{
    #region Variables

    [Header("Difficulty Settings")]
    public Difficulty[] difficultyList;
    public TMP_Text difficultyText;

    public TMP_Text recommendedText;
    private int selectedDifficultyIndex = 0;

    [Header("Mode Settings")]
    public TMP_Text modeText;
    private int selectedModeIndex = 0;

    #endregion

    [SerializeField] MainMenuManager _mainMenuManager;
    [SerializeField] GameObject _pivotStartSettings;

    [SerializeField] private EventReference openSceneTransitionSound;

    #region Unity Callbacks

    private void Awake()
    {
        CheckPlayerPrefs();
    }

    #endregion

    #region Public Methods

    public void ChangeSelectedDifficulty()
    {
        selectedDifficultyIndex = (selectedDifficultyIndex + 1) % difficultyList.Length;
        UpdateSelection(ref selectedDifficultyIndex, difficultyList.Select(d => d.DifficultyName).ToArray(), difficultyText);
        SetRecommendedText();
    }

    public void ChangeSelectedMode()
    {
        selectedModeIndex = (selectedModeIndex + 1) % Enum.GetValues(typeof(GameMode)).Length;
        UpdateSelection(ref selectedModeIndex, Enum.GetNames(typeof(GameMode)), modeText);
    }

    public void StartGame()
    {
        PlayerPrefs.SetString(Constant.DIFFICULTY_KEY, difficultyList[selectedDifficultyIndex].DifficultyName);
        PlayerPrefs.SetInt(Constant.MODE_KEY, selectedModeIndex);

        OpenSceneTransitionSound();

        _mainMenuManager.StartParty();
    }

    private void SetRecommendedText()
    {
        recommendedText.text = selectedDifficultyIndex switch
        {
            0 => "Best for 1-2 players",
            1 => "Best for 3-4 players",
            _ => "Best for 4 players",
        };
    }

    public void GoToMenu() {
        _mainMenuManager.ActiveButtons();
        _pivotStartSettings.SetActive(false);
    }

    #endregion

    #region Private Methods

    private void UpdateSelection(ref int selectedIndex, string[] items, TMP_Text displayText)
    {
        displayText.text = items[selectedIndex];
    }

    private void CheckPlayerPrefs()
    {
        string savedDifficulty = PlayerPrefs.GetString(Constant.DIFFICULTY_KEY, null);
        GameMode savedMode = (GameMode)PlayerPrefs.GetInt(Constant.MODE_KEY, 0);

        // Update Difficulty
        int difficultyIndex = savedDifficulty != null 
            ? Array.FindIndex(difficultyList, d => d.DifficultyName == savedDifficulty) 
            : 0;

        selectedDifficultyIndex = difficultyIndex != -1 ? difficultyIndex : 0;
        UpdateSelection(ref selectedDifficultyIndex, difficultyList.Select(d => d.DifficultyName).ToArray(), difficultyText);
        SetRecommendedText();

        // Update Mode
        selectedModeIndex = (int)savedMode;
        UpdateSelection(ref selectedModeIndex, Enum.GetNames(typeof(GameMode)), modeText);
    }


    #endregion


    #region Sounds
    private void OpenSceneTransitionSound()
    {
        RuntimeManager.PlayOneShot(openSceneTransitionSound, transform.position);
    }
    #endregion
}