using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Message
{
    NoGold,
    Lava,
    PickaxeEvent,
    TaxeEvent,
    LavaEvent,
    GoblinEvent,
    ForgeEvent
}
public class StringManager : MonoBehaviour
{
    [Header("GameOver Message")]
    [SerializeField] string _goblinDeathCondition;
    [SerializeField] string _lavaDeathCondition;

    [Header("Event Message")]
    [SerializeField] string _pickaxeIssue;
    [SerializeField] string _taxeDay;
    [SerializeField] string _approachingLava;
    [SerializeField] string _goblinWave;
    [SerializeField] string _noForge;

    public static StringManager Instance; // A static reference to the TargetManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public string GetSentence(Message target)
    {
        return target switch
        {
            Message.NoGold => _goblinDeathCondition,
            Message.Lava => _lavaDeathCondition,
            Message.PickaxeEvent => _pickaxeIssue,
            Message.TaxeEvent => _taxeDay,
            Message.LavaEvent => _approachingLava,
            Message.GoblinEvent => _goblinWave,
            Message.ForgeEvent => _noForge,
            _ => null,
        };
    }
}
