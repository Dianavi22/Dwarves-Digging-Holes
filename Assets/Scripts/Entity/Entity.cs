using DG.Tweening;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IGrabbable
{
    protected Rigidbody _rb;

    public bool IsGrabbed { get; protected set; }
    public float recoveryTime = 0.5f;

    public FixedJoint Joint;
    public bool HasJoint => Joint != null;

    private Tween recoveryTween;

    public virtual void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        if (isGrabbed)
        {
            HandleTween();
            IsGrabbed = isGrabbed;
        }
        else
        {
            HandleTween();
            recoveryTween = DOVirtual.DelayedCall(recoveryTime, () => { IsGrabbed = isGrabbed; recoveryTween = null; });
        }

    }

    private void HandleTween()
    {
        if (recoveryTween != null)
        {
            recoveryTween.Kill();
            recoveryTween = null;
        }
    }

    public virtual void HandleDestroy()
    {
        Destroy(gameObject);
    }

    public GameObject GetGameObject() => gameObject;

    public Rigidbody GetRigidbody() => _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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