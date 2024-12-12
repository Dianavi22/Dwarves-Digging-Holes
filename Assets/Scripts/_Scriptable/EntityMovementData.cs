using UnityEngine;

[CreateAssetMenu(fileName = "New MovementData")]
public class EntityMovementData : ScriptableObject
{

    [Header("Run")]
    public float RunMaxSpeed; //Target speed we want the player to reach.
    public float RunAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
    public float RunDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
    public float VelocityPower;
    [Range(0.01f, 1f)]
    public float PushChariotSpeedReducer = 1f;

    [Header("Other Stats")]
    public float JumpForce;
    public float DashForce;
    public float FallMultiplier;

    private void OnValidate()
    {
        #region Variable Ranges
        RunAcceleration = Mathf.Clamp(RunAcceleration, 0.01f, RunMaxSpeed);
        RunDecceleration = Mathf.Clamp(RunDecceleration, 0.01f, RunMaxSpeed);
        #endregion
    }
}
