using UnityEngine;

[CreateAssetMenu(fileName = "New MovementData")]
public class EntityMovementData : ScriptableObject
{
    public float Speed;
    public float JumpForce;
    public float FallMultiplier;
}
