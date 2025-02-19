using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IGrabbable
{
    protected Rigidbody _rb;
    protected EntityMovement movements;
    public bool IsGrabbed { get; protected set; }

    [HideInInspector] public Player holdBy;
    [HideInInspector] public FixedJoint Joint;
    public bool HasJoint => Joint != null;

    private Tween recoveryTween;

    [HideInInspector] public bool CanMoveAfterGrab = true;

    public virtual bool HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        IsGrabbed = isGrabbed;
        if (isGrabbed)
        {
            holdBy = currentPlayer;
            CanMoveAfterGrab = false;
        }
        else
        {
            StartCoroutine(DefineCanMoveAfterGrab());
        }
        return true;
    }

    private IEnumerator DefineCanMoveAfterGrab()
    {
        yield return new WaitForFixedUpdate();
        while (Mathf.Abs(_rb.velocity.x) - GameManager.Instance.CurrentScrollingSpeed > movements.Stats.VelocityTresholdAfterThrow)
        {
            yield return null;
        }
        CanMoveAfterGrab = !IsGrabbed; // In case the entity is re grabbed
    }

    public virtual void HandleDestroy()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject() => gameObject;

    public Rigidbody GetRigidbody() => _rb;
    public EntityMovement GetMovement() => movements;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        movements = GetComponent<EntityMovement>();
    }

    public void CreateFixedJoin(Rigidbody obj)
    {
        if (Joint != null) return;
        _rb.mass = 20f;
        Joint = gameObject.AddComponent<FixedJoint>();
        Joint.connectedBody = obj;
    }

    public void EmptyFixedJoin()
    {
        _rb.mass = 1f;
        Destroy(Joint);
        Joint = null;
    }
}