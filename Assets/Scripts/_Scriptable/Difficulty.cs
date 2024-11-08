using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty")]
public class Difficulty : ScriptableObject
{
    [Header("General")]
    public float ScrollingSpeed;

    [Header("GoldChariot")]
    public float NbStartingGold;

    [Header("Pickaxe")]
    public float MiningSpeed;
    public float PickaxeDurability;
    public float MaxNumberPickaxe;
}
