using UnityEngine;

public class Player : Entity
{
    private PlayerActions actions;
    private PlayerHealth health;
    private PlayerFatigue fatigue;
    private UserInput input;

    public Camera playerCamera;

    [SerializeField] private GameObject playerCameraPrefab;
    
    [SerializeField] private Animator animator;
    //[HideInInspector] public bool IsCarried = false;

    [HideInInspector] public bool HasCompletedLevel = false;

    protected override void Awake()
    {
        base.Awake();
        actions = GetComponent<PlayerActions>();
        health = GetComponent<PlayerHealth>();
        fatigue = GetComponent<PlayerFatigue>();
        input = GetComponent<UserInput>();
        playerCamera = Instantiate(playerCameraPrefab).GetComponent<Camera>();
        playerCamera.gameObject.GetComponent<FollowPlayer>().playerToFollow = transform;
    }

    public PlayerActions GetActions() => actions;
    public PlayerHealth GetHealth() => health;
    public PlayerFatigue GetFatigue() => fatigue;
    public UserInput GetInput() => input;
    public Animator GetAnimator() => animator;

    public override void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        base.HandleCarriedState(currentPlayer, isGrabbed);
        ((PlayerMovements) movements).forceDetachFunction = currentPlayer.GetActions().ForceDetach;
    }

    public bool CanDoAnything()
    {
        if (GameManager.Instance.isInMainMenu) return true;
        return !GameManager.Instance.isGameOver && !UIPauseManager.Instance.isPaused;
    }

    public override void HandleDestroy()
    {
        health.DeathPlayer();
    }
}