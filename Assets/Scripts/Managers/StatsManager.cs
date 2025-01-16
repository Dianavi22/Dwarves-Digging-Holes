using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private List<StatNameToText> _statisticsText;

    [SerializeField] private Texture2D _image;

    private readonly Dictionary<Player, Dictionary<string, int>> _playerStatistics = new();
    private List<string> _statNames;

    public bool GameStopped { get; private set; } = false;
    public static StatsManager Instance { get; private set; } // Singleton instance

    #region Initialization

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _statNames = GetAllStatNames();
        InitializePlayerStatistics();
        RestoreRenderTexture();
    }

    private void InitializePlayerStatistics()
    {
        foreach (Player player in GamePadsController.Instance.PlayerList)
        {
            _playerStatistics[player] = CreateStatDictionary();
        }
    }

    private Dictionary<string, int> CreateStatDictionary()
    {
        return _statNames.ToDictionary(stat => stat, _ => 0);
    }

    #endregion

    #region Gameplay Methods

    public void IncrementStatistic(Player player, StatsName statName, int value)
    {
        if (GameStopped || !_playerStatistics.ContainsKey(player)) return;

        if (_playerStatistics[player].TryGetValue(statName.ToString(), out var current))
        {
            _playerStatistics[player][statName.ToString()] = current + value;
        }
    }

    #endregion

    #region Statistics Analysis

    private Dictionary<Player, int> GetTopPlayersInStat(string statName)
    {
        List<KeyValuePair<Player, Dictionary<string, int>>> topPlayers = _playerStatistics
            .Where(kv => kv.Value.TryGetValue(statName, out var statValue) && statValue > 0)
            .OrderByDescending(kv => kv.Value[statName])
            .ToList();

        if (topPlayers.Count == 0) return null;

        if (topPlayers.Count == 1 || topPlayers[0].Value[statName] != topPlayers[1].Value[statName])
        {
            return new Dictionary<Player, int> { { topPlayers[0].Key, topPlayers[0].Value[statName] } };
        }

        return null;
    }

    private void DisplayEndGameStats()
    {
        foreach (var statName in _statNames)
        {
            Dictionary<Player, int> topPlayerData = GetTopPlayersInStat(statName);
            if (topPlayerData != null)
            {
                StatNameToText statText = _statisticsText.FirstOrDefault(st => st.Key.ToString() == statName);
                if (statText != null)
                {
                    // Camera
                    Camera playerCamera = topPlayerData.Keys.FirstOrDefault().playerCamera;
                    
                    playerCamera.enabled = true;
                    playerCamera.targetTexture = statText.renderTexture;
                    TakeScreenshot(playerCamera, statText.renderTexture);
                    playerCamera.enabled = false;


                    statText.Value.text = topPlayerData.Values.FirstOrDefault().ToString();
                }
            }
        }
    }

    public void EndGame()
    {
        GameStopped = true;
        DisplayEndGameStats();
    }

    #endregion

    #region Helpers

    private List<string> GetAllStatNames()
    {
        return Enum.GetNames(typeof(StatsName)).ToList();
    }

    private void RestoreRenderTexture() {
        foreach (StatNameToText item in _statisticsText)
        {
             Graphics.Blit(_image, item.renderTexture);
        }
    }

    private void TakeScreenshot(Camera targetCamera, RenderTexture renderTexture)
    {
        // Save the current RenderTexture
        RenderTexture currentRT = RenderTexture.active;

        // Set the active RenderTexture to the one used by the camera
        RenderTexture.active = renderTexture;

        // Render the camera's view
        targetCamera.Render();

        // Create a Texture2D to store the screenshot
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Read the pixels from the RenderTexture
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();


        // Restore the active RenderTexture
        RenderTexture.active = currentRT;
    }

    #endregion
}

[Serializable]
public class StatNameToText
{
    public StatsName Key;
    public TMP_Text Value;
    public RenderTexture renderTexture;
}
