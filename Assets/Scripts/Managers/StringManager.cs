using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Message
{
    NoGold,
    Lava,

    PickaxeEvent,
    PickaxeEventDesc,
    TaxEvent,
    TaxEventDesc,
    LavaEvent,
    LavaEventDesc,
    GoblinEvent,
    GoblinEventDesc,
    ForgeEvent,
    ForgeEventDesc,
}

public enum LevelCompleteMessage
{
    SuccessTitle,
    SuccessDesc,
    FailTitle,
    FailDesc,

    LittleGold,
    AverageGold,
    GreatGold,
    ExtraGold,
    GoldMountain
}

public class StringManager : MonoBehaviour
{
    [Header("GameOver Message")]
    [SerializeField] string _goblinDeathCondition;
    [SerializeField] string _lavaDeathCondition;

    [Header("Event Message")]
    [SerializeField] string _pickaxeIssue;
    [SerializeField] string _pickaxeIssueDescription;
    [Space]
    [SerializeField] string _taxDay;
    [SerializeField] string _taxDayDescription;
    [Space]
    [SerializeField] string _approachingLava;
    [SerializeField] string _approachingLavaDescription;
    [Space]
    [SerializeField] string _goblinWave;
    [SerializeField] string _goblinWaveDescription;
    [Space]
    [SerializeField] string _noForge;
    [SerializeField] string _noForgeDescription;

    [Header("Level Complete")]
    [SerializeField] string _congratsTitle;
    [SerializeField] string _congratsDescription;
    [Space]
    [SerializeField] string _missingChariotTitle;
    [SerializeField] string _missingChariotDescription;
    [Space]
    [SerializeField] string _endingCountingGoldDescription_1;
    [SerializeField] string _endingCountingGoldDescription_2;
    [SerializeField] string _endingCountingGoldDescription_3;
    [SerializeField] string _endingCountingGoldDescription_4;
    [SerializeField] string _endingCountingGoldDescription_5;


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
            Message.PickaxeEventDesc => _pickaxeIssueDescription,
            Message.TaxEvent => _taxDay,
            Message.TaxEventDesc => _taxDayDescription,
            Message.LavaEvent => _approachingLava,
            Message.LavaEventDesc => _approachingLavaDescription,
            Message.GoblinEvent => _goblinWave,
            Message.GoblinEventDesc => _goblinWaveDescription,
            Message.ForgeEvent => _noForge,
            Message.ForgeEventDesc => _noForgeDescription,
            _ => null,
        };
    }

    public string GetLevelCompleteSentence(LevelCompleteMessage target)
    {
        return target switch
        {
            LevelCompleteMessage.SuccessTitle => _congratsTitle,
            LevelCompleteMessage.SuccessDesc => _congratsDescription,
            LevelCompleteMessage.FailTitle => _missingChariotTitle,
            LevelCompleteMessage.FailDesc => _missingChariotDescription,

            LevelCompleteMessage.LittleGold => _endingCountingGoldDescription_1,
            LevelCompleteMessage.AverageGold => _endingCountingGoldDescription_2,
            LevelCompleteMessage.GreatGold => _endingCountingGoldDescription_3,
            LevelCompleteMessage.ExtraGold => _endingCountingGoldDescription_4,
            LevelCompleteMessage.GoldMountain => _endingCountingGoldDescription_5,
            _ => null,
        };
    }
}
