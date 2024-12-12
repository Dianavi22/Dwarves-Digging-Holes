using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [HideInInspector] public bool canFlip = true;

    protected bool isGrounded;
    protected bool flip;

    protected bool CanMove = true;
    protected Entity GetBase;
    protected EntityMovementData Stats;

    private float horizontalInput;

    private Rigidbody RB => GetBase.GetRigidbody();

    public void SetStats(EntityMovementData newStats)
    {
        Stats = newStats;
    }

    private void FixedUpdate()
    {
        HandleGround();
        HandleMovement();
        HandleJumpPhysics();
    }

    protected virtual void Update()
    {
        HandleFlip();
    }

    protected virtual void HandleMovement()
    {
        if (!CanMove) return;

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = horizontalInput * Stats.RunMaxSpeed;

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

    protected void HandleGround() {
        //Grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (!isGrounded)
        {
            Vector3 velocity = Vector3.zero;
            velocity.y = -2f;
            velocity.y += -9.81f * Time.deltaTime;
            RB.AddForce(velocity * Time.deltaTime);
        }
    }

    protected virtual void HandleJumpPhysics()
    {
        if (!isGrounded)
        {
            // Faster falling
            if (RB.velocity.y < 0)
            {
                RB.velocity += (Stats.FallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
            }
        }
    }

    private void HandleFlip()
    {
        if (GameManager.Instance.isGameOver) return;
        if ((horizontalInput < 0 && flip && canFlip) || (horizontalInput > 0 && !flip && canFlip))
        {
            flip = !flip;
            transform.rotation = Quaternion.Euler(0, flip ? 180 : 0, 0);
        }
    }

    public virtual void Move(float horizontal)
    {
        horizontalInput = horizontal;
    }

    public virtual void Jump()
    {
        float force = Stats.JumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector3.up * force, ForceMode.Impulse);
    }
}
