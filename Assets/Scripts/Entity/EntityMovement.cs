using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    protected bool isGrounded;
    protected bool flip;
    protected float horizontalInput;
    protected Vector3 velocity;

    protected Entity GetBase;
    protected EntityMovementData Stats;

    [HideInInspector] public bool canFlip = true;

    protected virtual void Update()
    {
        HandleGround();
        HandleMovement();
        HandleJumpPhysics();
        HandleFlip();
    }

    protected virtual void HandleMovement()
    {
        if (horizontalInput != 0)
        {
            Vector3 newVelocity = new(horizontalInput * Stats.Speed, GetBase.GetRigidbody().velocity.y, 0f);
            GetBase.GetRigidbody().velocity = newVelocity;
        }
    }

    protected void HandleGround() {
        //Grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f); ;
        if (!isGrounded)
        {
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

    protected void HandleFlip()
    {
        if (GameManager.Instance.isGameOver) return;
        if (((horizontalInput < 0 && flip) || (horizontalInput > 0 && !flip)) && canFlip)
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
