using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance; // A static reference to the GameManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void GameStarted()
    {

    }

    public void GameOver()
    {
        Time.timeScale = 0;
    }

}
