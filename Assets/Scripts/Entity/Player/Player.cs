using UnityEngine;

public class Player : Entity
{
    private PlayerActions actions;
    private PlayerHealth health;
    private PlayerFatigue fatigue;
    private PlayerMovements playerMovements;
    private UserInput input;

    private PlayerModels _model;

    public Camera playerCamera;
    public int playerIndex;

    [SerializeField] private GameObject playerCameraPrefab;

    private Animator _animator;
    //[HideInInspector] public bool IsCarried = false;

    [HideInInspector] public bool HasCompletedLevel = false;

    protected override void Awake()
    {
        base.Awake();
        actions = GetComponent<PlayerActions>();
        health = GetComponent<PlayerHealth>();
        fatigue = GetComponent<PlayerFatigue>();
        playerMovements = GetComponent<PlayerMovements>();
        input = GetComponent<UserInput>();
        playerCamera = Instantiate(playerCameraPrefab).GetComponent<Camera>();
        playerCamera.gameObject.GetComponent<FollowPlayer>().playerToFollow = transform;
    }

    public PlayerActions GetActions() => actions;
    public PlayerHealth GetHealth() => health;
    public PlayerFatigue GetFatigue() => fatigue;
    public UserInput GetInput() => input;
    public Animator GetAnimator() => _model.GetAnimator();
    public PlayerModels GetModelRef()
    {
        if (!_model)
        {
            print("INSTANTIATE NEW PLAYER MODEL - DEBUG CRASH");
            _model = Instantiate(GamePadsController.Instance.m_PlayerModels[playerIndex], transform);
        }
        
        return _model;
    }

    public PlayerMovements GetPlayerMovements() => playerMovements;

    public void SetModelRef(PlayerModels model)
    {
        _model = model;
    }

    public override bool HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        bool canBeCarried = base.HandleCarriedState(currentPlayer, isGrabbed);
        if (!canBeCarried) return false;

        ((PlayerMovements) movements).forceDetachFunction = currentPlayer.GetActions().ForceDetach;

        GetAnimator().SetBool("isGrabbed", isGrabbed);
        currentPlayer.GetAnimator().SetBool("isGrabbing", isGrabbed);

        return canBeCarried;
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