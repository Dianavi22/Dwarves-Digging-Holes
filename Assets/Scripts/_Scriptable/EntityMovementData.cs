using UnityEngine;

[CreateAssetMenu(fileName = "New MovementData")]
public class EntityMovementData : ScriptableObject
{

    [Header("Run")]
    public float RunMaxSpeed; //Target speed we want the player to reach.
    public float RunAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
    [HideInInspector] public float RunAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float RunDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
    [HideInInspector] public float RunDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    public float VelocityPower;
    [Space(10)]
    [Range(0.01f, 1)] public float AccelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] public float DeccelInAir;
    public bool DoConserveMomentum;

    [Header("Other Stats")]
    public float JumpForce;
    public float FallMultiplier;

    private void OnValidate()
    {
        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;
        RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;

        #region Variable Ranges
        RunAcceleration = Mathf.Clamp(RunAcceleration, 0.01f, RunMaxSpeed);
        RunDecceleration = Mathf.Clamp(RunDecceleration, 0.01f, RunMaxSpeed);
        #endregion
    }
}
