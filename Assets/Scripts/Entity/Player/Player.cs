using DG.Tweening;
using UnityEngine;

/**
 * @todo Need to better instanciate this for better performance
 * For now - each child will do the Awake. Better if can use SerializeField only once where each child can have reference to
 */
public class Player : MonoBehaviour, IGrabbable
{
    private PlayerMovements movements;
    private PlayerActions actions;
    private PlayerHealth health;
    private PlayerFatigue fatigue;
    private UserInput input;
    private Rigidbody rb;

    private FixedJoint _joint;

    public bool IsCarried {  get; private set; }

    private void Awake()
    {
        movements = GetComponent<PlayerMovements>();
        actions = GetComponent<PlayerActions>();
        health = GetComponent<PlayerHealth>();
        fatigue = GetComponent<PlayerFatigue>();
        input = GetComponent<UserInput>();
        rb = GetComponent<Rigidbody>();
    }

    public PlayerMovements GetMovement() => movements;
    public PlayerActions GetActions() => actions;
    public PlayerHealth GetHealth() => health;
    public PlayerFatigue GetFatigue() => fatigue;
    public UserInput GetInput() => input;
    public Rigidbody GetRigidbody() => rb;

    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        movements.forceDetachFunction = currentPlayer.GetActions().ForceDetach;
        IsCarried = isGrabbed;

        //if (!isGrabbed)
        //{
        //    DOVirtual.DelayedCall(0.25f, () => { movements.canStopcarried = true; });
        //}
    }

    public void CreatePlayerFixedJoin(Rigidbody obj)
    {
        if (_joint != null)
        {
            Debug.Log(_joint);
            return;
        }
        rb.mass = 20f;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = obj;
    }

    public void EmptyPlayerFixedJoin()
    {
        rb.mass = 1f;
        Destroy(_joint);
        _joint = null;
    }
}