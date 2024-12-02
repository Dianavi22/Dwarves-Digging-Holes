using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private int scoreToAddTimer = 10;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] TMP_Text _bestScoreTxt;
    [SerializeField] TMP_Text _newRecordText;
    [SerializeField] ShakyCame _shakyCame;
    [SerializeField] ParticleSystem _newRecordPart;

    private int _bestScore;


    private int score = 0;
    private int _scoreFixed;
    private bool _isNewRecord = false;

    void Start()
    {
        InvokeRepeating(nameof(AddScoreTimer), 1f, 1f);
        _bestScoreTxt.text = PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY).ToString();
    }

   
    private void UpdateScore()
    {
        scoreText.text = score.ToString();
        if(int.Parse(_bestScoreTxt.text) < score && !_isNewRecord)
        {
            _isNewRecord = true;
            NewRecord();
        }

    }

    private void NewRecord()
    {
        print("New Record");
        _newRecordText.gameObject.SetActive(true);
        _bestScoreTxt.gameObject.SetActive(false);
        _shakyCame.ShakyCameCustom(1, 0.3f);
        _newRecordPart.Play();
    }

    public void AddScoreOnce(int scoreToAdd)
    {
        if (!_gameManager.isGameOver)
        {
            score += scoreToAdd;
            UpdateScore();
        }
    }

    private void AddScoreTimer()
    {
        if (!_gameManager.isGameOver)
        {
            score += scoreToAddTimer;
            UpdateScore();
        }
    }

    public bool CheckBestScore()
    {
        _bestScore = PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY);
        if (score > _bestScore)
        {
            PlayerPrefs.SetInt(Utils.BEST_SCORE_KEY, score);
            return true;
        }
        return false;
    }
}
