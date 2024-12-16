using UnityEngine;

[CreateAssetMenu(fileName = "New FatigueData")]
public class PlayerFatigueData : ScriptableObject
{
    public float MaxFatigue;
    public float RegenDelay;
    public float RegenByFrame;
    public float ActionReducer;
}
