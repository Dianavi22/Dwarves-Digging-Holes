using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty")]
public class Difficulty : ScriptableObject
{
    [Header("General")]
    public string DifficultyName;
    public float ScrollingSpeed;
    public EntityMovementData GoblinStats;

    [Header("Player")]
    public EntityMovementData PlayerStats;

    public PlayerFatigueData MiningFatigue;
    public PlayerFatigueData PushCartFatigue;

    [Header("GoldChariot")]
    public int NbStartingGold;

    [Header("Pickaxe")]
    public float MiningSpeed;
    public int PickaxeDurability;
    public int MaxNumberPickaxe;
}
