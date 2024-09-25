using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Target
{
    GoldChariot,
    RespawnPoint
}

public class TargetManager : MonoBehaviour
{
    //[SerializeField] private GameObject player;
    [SerializeField] private GameObject goldChariot;
    [SerializeField] private GameObject respawnPoint;

    public static TargetManager Instance; // A static reference to the TargetManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public GameObject GetGameObject(Target target)
    {
        return target switch
        {
            //Target.Player => player,
            Target.GoldChariot => goldChariot,
            Target.RespawnPoint => respawnPoint,
            _ => null,
        };
    }
}
