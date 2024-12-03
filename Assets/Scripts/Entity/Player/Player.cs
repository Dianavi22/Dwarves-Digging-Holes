using UnityEngine;

public class Player : Entity
{
    private PlayerMovements movements;
    private PlayerActions actions;
    private PlayerHealth health;
    private PlayerFatigue fatigue;
    private UserInput input;
    
    [SerializeField]
    private Animator animator;

    private FixedJoint _joint;

    public bool HasJoint => _joint != null;
    [HideInInspector] public bool IsCarried = false;

    protected override void Awake()
    {
        base.Awake();
        movements = GetComponent<PlayerMovements>();
        actions = GetComponent<PlayerActions>();
        health = GetComponent<PlayerHealth>();
        fatigue = GetComponent<PlayerFatigue>();
        input = GetComponent<UserInput>();
    }

    public PlayerMovements GetMovement() => movements;
    public PlayerActions GetActions() => actions;
    public PlayerHealth GetHealth() => health;
    public PlayerFatigue GetFatigue() => fatigue;
    public UserInput GetInput() => input;
    public Animator GetAnimator() => animator;

    public override void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        base.HandleCarriedState(currentPlayer, isGrabbed);
        movements.forceDetachFunction = currentPlayer.GetActions().ForceDetach;
    }

    public void CreatePlayerFixedJoin(Rigidbody obj)
    {
        if (_joint != null) return;
        _rb.mass = 20f;
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = obj;
    }

    public void EmptyPlayerFixedJoin()
    {
        _rb.mass = 1f;
        Destroy(_joint);
        _joint = null;
    }

    public override void HandleDestroy()
    {
        health.DeathPlayer();
    }
}