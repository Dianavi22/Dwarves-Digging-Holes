using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameSettingsSelection : MonoBehaviour
{

    #region Difficulty Variables
    public Difficulty[] difficultyList;
    public TMP_Text difficultyText;
    private Difficulty selectedDifficulty;
    private int selectedDifficultyIndex = 0;

    #endregion

    #region Mode Variables
    private int selectedModeIndex = 0;
    private GameMode selectedMode;
    public TMP_Text modeText;
    #endregion

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        CheckPlayerPrefs();
    }

    public void ChangeSelected()
    {
        selectedDifficultyIndex = (selectedDifficultyIndex + 1) % difficultyList.Length;
        ChangeDifficulty(selectedDifficultyIndex);
    }

    public void ChangeSelectedMode()
    {
        selectedModeIndex = (selectedModeIndex + 1) %  Enum.GetValues(typeof(GameMode)).Length;
        ChangeMode((GameMode) selectedModeIndex);
    }

    private void ChangeDifficulty(int index)
    {
        if(selectedDifficultyIndex != index) {
            selectedDifficultyIndex = index;
        }
        selectedDifficulty = difficultyList[index];
        difficultyText.text = selectedDifficulty.DifficultyName;
    }

    private void ChangeMode(GameMode gameMode) {
        selectedMode = gameMode;
        modeText.text = Enum.GetNames(typeof(GameMode))[selectedModeIndex];
    }

    private void CheckPlayerPrefs()
    {
        string difficultyName = PlayerPrefs.GetString(Constant.DIFFICULTY_KEY, null);
        GameMode gameMode = (GameMode) PlayerPrefs.GetInt(Constant.MODE_KEY, 0);

        if (difficultyName != null)
        {
            int index = Array.FindIndex(difficultyList, x => x.DifficultyName == difficultyName);
            ChangeDifficulty(index != -1 ? index : 0);
        }
        else
        {
            ChangeDifficulty(0);
        }

        ChangeMode(gameMode);
    }

    public void StartGame()
    {
        PlayerPrefs.SetString(Constant.DIFFICULTY_KEY, selectedDifficulty.DifficultyName);
        PlayerPrefs.SetInt(Constant.MODE_KEY, (int) selectedMode);

        SceneManager.LoadScene(2);
    }
}
