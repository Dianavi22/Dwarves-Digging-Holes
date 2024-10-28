using DG.Tweening;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * @todo Need to better instanciate this for better performance
 * For now - each child will do the Awake. Better if can use SerializeField only once where each child can have reference to
 */
public class Player : MonoBehaviour
{
    protected PlayerMovements movements;
    protected PlayerActions actions;
    protected PlayerHealth health;
    protected PlayerFatigue fatigue;
    protected UserInput input;
    protected Rigidbody rb;

    private FixedJoint _joint;

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

    public void HandleCarriedState(bool isGrabbed)
    {
        if (isGrabbed)
        {
            movements.carried = true;
        }
        else
        {
            DOVirtual.DelayedCall(0.25f, () => { movements.canStopcarried = true; });
        }

        actions.carried = isGrabbed;
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
