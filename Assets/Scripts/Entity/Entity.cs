using UnityEngine;

public abstract class Entity : MonoBehaviour, IGrabbable
{
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float lifePoint = 3f;
    protected Rigidbody _rb;

    public bool IsGrabbed { get; protected set; }

    public virtual void HandleCarriedState(Player currentPlayer, bool isGrabbed) {
        IsGrabbed = isGrabbed;
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