using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCompleteManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject mainMenuButton;
    
    void Start()
    {
        Debug.Log("LevelCompleteManager Start");
    }

    void LevelComplete()
    {
        StartCoroutine(GameManager.Instance.LevelComplete(this));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LevelComplete();
        }
    }
}
