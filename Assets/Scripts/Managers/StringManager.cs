using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DeathMessage
{
    NoGold,
    Lava
}
public class StringManager : MonoBehaviour
{
    [SerializeField] string _goblinDeathCondition;
    [SerializeField] string _lavaDeathCondition;

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

    public string GetDeathMessage(DeathMessage target)
    {
        return target switch
        {
            //Target.Player => player,
            DeathMessage.NoGold => _goblinDeathCondition,
            DeathMessage.Lava => _lavaDeathCondition,
            _ => null,
        };
    }
}
