using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty")]
public class Difficulty : ScriptableObject
{
    [Header("General")]
    public float ScrollingSpeed;

    [Header("Player")]
    public float PlayerMiningFatigue;
    public float PlayerPushFatigue;

    [Header("GoldChariot")]
    public int NbStartingGold;

    [Header("Pickaxe")]
    public float MiningSpeed;
    public int PickaxeDurability;
    public int MaxNumberPickaxe;
}
