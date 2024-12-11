using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    protected bool isGrounded;
    protected bool flip;

    protected bool CanMove = true;
    protected Entity GetBase;
    protected EntityMovementData Stats;

    private float horizontalInput;

    public void SetStats(EntityMovementData newStats)
    {
        Stats = newStats;
    }

    [HideInInspector] public bool canFlip = true;

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

        Rigidbody RB = GetBase.GetRigidbody();

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = horizontalInput * Stats.RunMaxSpeed;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player
        //Calculate force along x-axis to apply to thr player

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? Stats.RunAcceleration : Stats.RunDecceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, Stats.VelocityPower) * Mathf.Sign(speedDif);

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector3.right, ForceMode.Force);
    }

    protected void HandleGround() {
        //Grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (!isGrounded)
        {
            Vector3 velocity = Vector3.zero;
            velocity.y = -2f;
            velocity.y += -9.81f * Time.deltaTime;
            GetBase.GetRigidbody().AddForce(velocity * Time.deltaTime);
        }
    }

    protected virtual void HandleJumpPhysics()
    {
        if (!isGrounded)
        {
            // Faster falling
            if (GetBase.GetRigidbody().velocity.y < 0)
            {
                GetBase.GetRigidbody().velocity += (Stats.FallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
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
        GetBase.GetRigidbody().AddForce(Vector3.up * Stats.JumpForce, ForceMode.Impulse);
    }
}
