using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private int scoreToAddTimer = 10;
    [SerializeField] TMP_Text _bestScoreTxt;

    [Header("Break New Record")]
    [SerializeField] TMP_Text _newRecordText;
    [SerializeField] ParticleSystem _newRecordPart;

    private int score = 0;
    public int ScoreCounter
    {
        get => score;
        set
        {
            if (GameManager.Instance.isGameOver) return;
            score = value;
            UpdateScore();
        }
    }
    private bool _isNewRecord = false;

    void Start()
    {
        InvokeRepeating(nameof(AddScoreTimer), 1f, 1f);
        _bestScoreTxt.text = PlayerPrefs.GetInt(Constant.BEST_SCORE_KEY).ToString();
    }

    private void UpdateScore()
    {
        scoreText.text = score.ToString();
        if(int.Parse(_bestScoreTxt.text) < score && !_isNewRecord && int.Parse(_bestScoreTxt.text) != 0)
        {
            _isNewRecord = true;
            NewRecord();
        }

    }

    private void NewRecord()
    {
        _newRecordText.gameObject.SetActive(true);
        _bestScoreTxt.gameObject.SetActive(false);
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(1, 0.3f);
        _newRecordPart.Play();
    }

    private void AddScoreTimer()
    {
        ScoreCounter += scoreToAddTimer;
    }

    public bool CheckBestScore()
    {
        int bestScore = PlayerPrefs.GetInt(Constant.BEST_SCORE_KEY);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(Constant.BEST_SCORE_KEY, score);
            return true;
        }
        return false;
    }
}
