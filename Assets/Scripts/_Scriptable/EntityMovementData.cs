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

    [Header("Gravity Scaller")]
    [Tooltip("Simple gravity multiplier when falling")] 
    public float BasicFalling;
    [Tooltip("When holding down control, add gravity")] 
    public float FastFalling;
    [Tooltip("When releasing early the jump, add extra gravity")] 
    public float JumpCut;
    [Tooltip("To add a floaty effect when approaching the apex of the jump by reducing gravity"), Range(0f, 1)] 
    public float JumpHangAir;

    [Header("Jump")]
    public float JumpHangTimeTreshold;
    public float JumpHeight;
    public float JumpTimeToApex;
    [HideInInspector] public float JumpForce;

    [Header("Other Stats")]
    public float DashForce;
    public float VelocityTresholdAfterThrow;

    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        float gravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);
        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        JumpForce = Mathf.Abs(gravityStrength) * JumpTimeToApex;

        #region Variable Ranges
        RunAcceleration = Mathf.Clamp(RunAcceleration, 0.01f, RunMaxSpeed);
        RunDecceleration = Mathf.Clamp(RunDecceleration, 0.01f, RunMaxSpeed);
        #endregion
    }
}
