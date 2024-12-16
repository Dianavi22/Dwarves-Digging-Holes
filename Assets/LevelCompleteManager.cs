using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCompleteManager : MonoBehaviour
{
    public float levelDuration = 60f;
    public GameObject canvas;
    public GameObject mainMenuButton;
    public GameObject blockSpawner;
    public GameObject lavaGFX;

    private float elapsedTime;
    private bool gameIsStarted;
    
    private void Start()
    {
        elapsedTime = 0f;
    }
    
    private void Update()
    {
        if (gameIsStarted)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log("progress: " + Mathf.Round(elapsedTime/levelDuration * 100) + "%");
            
            if (elapsedTime >= levelDuration || Input.GetKeyDown(KeyCode.T))
            {
                LevelComplete();
            }
        }
    }

    public void StartGame()
    {
        gameIsStarted = true;
    }

    private void LevelComplete()
    {
        gameIsStarted = false;
        StartCoroutine(GameManager.Instance.LevelComplete(this));
    }
}
