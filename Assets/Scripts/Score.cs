using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private int scoreToAddTimer = 10;
    [SerializeField] private GameManager _gameManager;

    private int score = 0;
    private int _scoreFixed;

    void Start()
    {
        InvokeRepeating(nameof(AddScoreTimer), 1f, 1f);
    }

   
    private void UpdateScore()
    {
            scoreText.text = score.ToString();
    }

    public void AddScoreOnce(int scoreToAdd)
    {
        if (!_gameManager.isGameOver)
        {
            print("AddScoreOnce");
            score += scoreToAdd;
            UpdateScore();
        }
    }

    private void AddScoreTimer()
    {
        if (!_gameManager.isGameOver)
        {
            print("AddScoreTimer");
            score += scoreToAddTimer;
            UpdateScore();
        }
    }

    public bool CheckBestScore()
    {
        int bestScore = PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(Utils.BEST_SCORE_KEY, score);
            return true;
        }
        return false;
    }
}
