using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    protected PlayerMovements movements;
    protected PlayerActions actions;
    protected PlayerHealth health;
    protected PlayerFatigue fatigue;

    private void Awake()
    {
        movements = GetComponent<PlayerMovements>();
        actions = GetComponent<PlayerActions>();
        health = GetComponent<PlayerHealth>();
        fatigue = GetComponent<PlayerFatigue>();
    }

    public PlayerMovements GetMovement() => movements;
    public PlayerActions GetActions() => actions;
    public PlayerHealth GetHealth() => health;
    public PlayerFatigue GetFatigue() => fatigue;
}
