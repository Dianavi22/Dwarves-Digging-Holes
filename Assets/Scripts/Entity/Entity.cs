using DG.Tweening;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IGrabbable
{
    protected Rigidbody _rb;

    public bool IsGrabbed { get; protected set; }
    public float recoveryTime = 0.5f;

    public virtual void HandleCarriedState(Player currentPlayer, bool isGrabbed) {
        if(isGrabbed) {
            IsGrabbed = isGrabbed;
        }
        else {
            DOVirtual.DelayedCall(recoveryTime, () =>  IsGrabbed = isGrabbed);
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
}