using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Add global reference from the editor
    [SerializeField] private GoldChariot goldChariot;
    [SerializeField] private RespawnPoint respawnPoint;
    [SerializeField] private ShakyCame shakyCame;
    [SerializeField] private TypeSentence typeSentence;
    [SerializeField] private Score score;
    [SerializeField] private Lava lava;
    [SerializeField] private Tuto tuto;

    public static TargetManager Instance; // A static reference to the TargetManager instance
    void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
    }

    // Then add the new reference into this list
    private List<MonoBehaviour> GetTargetList()
    {
        List<MonoBehaviour> list = new()
        {
            goldChariot,
            respawnPoint,
            shakyCame,
            typeSentence,
            score,
            lava,
            tuto
        };
        return list;
    }

    public T GetGameObject<T>() where T : MonoBehaviour
    {
        foreach (MonoBehaviour target in GetTargetList())
        {
            if (target.GetType() == typeof(T))
                return target as T;
        }
        return default;
    }
}
