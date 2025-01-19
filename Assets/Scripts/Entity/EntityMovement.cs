using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EntityMovement : MonoBehaviour
{
    [HideInInspector] public bool canFlip = true;

    [HideInInspector] public bool isGrounded;
    protected bool flip;

    protected bool IsPerformingJump = false;
    protected bool CanMove = true;
    protected Entity GetBase;
    public EntityMovementData Stats;

    protected Vector2 _moveInput;

    private Rigidbody RB => GetBase.GetRigidbody();

    public void SetStats(EntityMovementData newStats)
    {
        Stats = newStats;
    }

    protected void FixedUpdate()
    {
      
            if (GameManager.Instance.isGameOver)
            {
                RB.velocity = Vector3.zero;
                RB.angularVelocity = Vector3.zero;
                RB.isKinematic = true;
            }

            DefineGroundState();
            HandleMovement();

            //Much higher gravity if holding down
            if (!isGrounded && RB.velocity.y < 0 && _moveInput.y < 0)
                GravityScaler(Stats.FastFalling);

            // If low jump, fall faster 
            // Note: Dunno why but velocity.y on the chariot is > to 0
            else if (!isGrounded && RB.velocity.y > 0 && !IsPerformingJump)
                GravityScaler(Stats.JumpCut);

            //Reducing Gravity when reaching the apex of the jump
            else if (Mathf.Abs(RB.velocity.y) < Stats.JumpHangTimeTreshold)
                GravityScaler(Stats.JumpHangAir);

            //Simple falling
            else if (RB.velocity.y < 0)
                GravityScaler(Stats.BasicFalling);

            else
                GravityScaler(1f);
       
    }

    protected void Update()
    {
        HandleFlip();
    }

    protected void GravityScaler(float multiplier)
    {
        RB.AddForce(Physics.gravity.y * multiplier * Vector3.up);
    }

    protected void HandleMovement()
    {
        if (GameManager.Instance.isInMainMenu || !CanMove || !GetBase.CanMoveAfterGrab ) return;

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Stats.RunMaxSpeed;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? Stats.RunAcceleration : Stats.RunDecceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, Stats.VelocityPower) * Mathf.Sign(speedDif);

        //Debug.Log("targetSpeed: " + targetSpeed + " | speedDif: " + speedDif + " | accelRate: " + accelRate + " | mvt: " + movement);

        //Convert this to a vector and apply to rigidbody
        if (GetBase.HasJoint)
            GetBase.Joint.connectedBody.AddForce(movement * Stats.PushChariotSpeedReducer * Vector3.right, ForceMode.Acceleration);
        else
            RB.AddForce(movement * Vector3.right);
    }

    private void DefineGroundState()
    {
        List<Collider> hits = DRayCast.Cone(transform.position, Vector3.down, 27.5f, 1.1f, 3, ~0);
        isGrounded = hits.Count > 0;
    }

    private void HandleFlip()
    {
        if (GameManager.Instance.isGameOver) return;
        if ((_moveInput.x < 0 && flip && canFlip) || (_moveInput.x > 0 && !flip && canFlip))
        {
            flip = !flip;
            transform.rotation = Quaternion.Euler(0, flip ? 180 : 0, 0);
        }
    }

    #region Movement Action
    public void Move(Vector2 movementInput)
    {
        _moveInput = movementInput;
    }

    public void Jump()
    {
        float force = Stats.JumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    public void Dash()
    {
        float force = Stats.DashForce;
        if (RB.velocity.x < 0)
            force -= RB.velocity.x;

        Vector3 dashDirection = flip ? Vector3.right : Vector3.left;
        RB.AddForce(dashDirection * force, ForceMode.Impulse);
    }
    #endregion
}
