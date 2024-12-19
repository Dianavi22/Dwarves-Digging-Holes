using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private List<StatNameToText> _statisticsText;

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
       // InitializePlayerStatistics();
    }

    private void InitializePlayerStatistics()
    {
        foreach (var player in GamePadsController.Instance.PlayerList)
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
        var topPlayers = _playerStatistics
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
            var topPlayerData = GetTopPlayersInStat(statName);
            if (topPlayerData != null)
            {
                var statText = _statisticsText.FirstOrDefault(st => st.Key.ToString() == statName);
                if (statText != null)
                {
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

    #endregion
}

[Serializable]
public class StatNameToText
{
    public StatsName Key;
    public TMP_Text Value;
}
