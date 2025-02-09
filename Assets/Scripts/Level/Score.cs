using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class Score : MonoBehaviour
{
    public bool isStartScore = false;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private int scoreToAddTimer = 10;
    [SerializeField] TMP_Text _bestScoreTxt;

    [Header("Break New Record")]
    [SerializeField] TMP_Text _newRecordText;
    [SerializeField] ParticleSystem _newRecordPart;

    private int scoreNextStep = 200;

    private int score = 0;
    public int ScoreCounter
    {
        get => score;
        set
        {
            if (isStartScore)
            {
                if (GameManager.Instance.isGameOver) return;
                score = value;
                UpdateScore();
            }

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
        if (int.Parse(_bestScoreTxt.text) < score && !_isNewRecord && int.Parse(_bestScoreTxt.text) != 0)
        {
            _isNewRecord = true;
            NewRecord();
        }

    }

    private void NewRecord()
    {
        _newRecordText.gameObject.SetActive(true);
        _bestScoreTxt.gameObject.SetActive(false);
        TargetManager.Instance.GetGameObject<ShakyCame>().ShakyCameCustom(1, 0.3f);
        _newRecordPart.Play();
    }

    private void AddScoreTimer()
    {
        if(!GameManager.Instance._eventManager.enabled) return;
        ScoreCounter += scoreToAddTimer;
        if (ScoreCounter >= scoreNextStep)
        {
           GameManager.Instance.SetScrollingSpeed(GameManager.Instance.CurrentScrollingSpeed + 0.1f);
            scoreNextStep += 200;
        }
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
