using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private Dictionary<Player, Dictionary<string, int>> _playerStatistics = new();

    private List<string> statNames = new();


    public bool gameStopped = false;
    public static StatsManager Instance; // A static reference to the GameManager instance

    #region Preparation

    public void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        statNames = StatsName.GetAllFieldsFromStatsName();
        PrepareDict();
    }

    private void PrepareDict()
    {
        foreach (Player p in GamePadsController.Instance.PlayerList)
        {
            _playerStatistics[p] = PrepareNestedDict();
        }

    }

    private Dictionary<string, int> PrepareNestedDict()
    {
        return new Dictionary<string, int>(statNames.ToDictionary(stat => stat, stat => 0));
    }

    #endregion

    public void StatGoldMined(Player playerId)
    {
        IncrementStatistic(playerId, StatsName.GoldMined, 1);
    }

    private void IncrementStatistic(Player playerId, string statName, int value)
    {
        if (gameStopped) return;

        _playerStatistics[playerId][statName] += value;
    }

    private Player? GetTopIn(string statName)
    {
        var topPlayers = _playerStatistics
            .Where(kv => kv.Value.ContainsKey(statName) && kv.Value[statName] > 0)
            .OrderByDescending(kv => kv.Value[statName])
            .DistinctBy(kv => kv)
            .ToList();

        if (topPlayers.Any())
        {
            if (topPlayers.Count == 1 || topPlayers[0].Value[statName] != topPlayers[1].Value[statName])
            {
                return topPlayers.First().Key;
            }
        }

        return null;
    }

    public Dictionary<string, Player> EndGameStats()
    {
        Dictionary<string, Player> winners = new();
        foreach (string stat in statNames)
        {
            winners[stat] = GetTopIn(stat);
        }

        return winners;
    }

}

public static class StatsName
{
    public static string GoldMined = "GoldMined";

    public static List<string> GetAllFieldsFromStatsName()
    {
        return typeof(StatsName)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => field.GetValue(null)?.ToString())
            .ToList();
    }
}
