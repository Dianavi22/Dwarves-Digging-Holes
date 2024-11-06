using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private int scoreToAddTimer = 10;

    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(AddScoreTimer), 1f, 1f);
    }


    private void UpdateScore() {
        scoreText.text = score.ToString(); 
    }

    public void AddScoreOnce(int scoreToAdd) {
        score += scoreToAdd;
        UpdateScore();
    }

    private void AddScoreTimer() {
        score += scoreToAddTimer;
        UpdateScore();
    }
}
