using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [Header("Player Instance")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] private List<PlayerModels> m_PlayerModels = new();

    [Header("UI")]
    [SerializeField] private GameObject m_MainCanvas;
    [SerializeField] private GameObject m_HeadFatigueBarUI;

    [Header("Debug")]
    public bool IsDebugMode;
    [SerializeField, Range(1, 4)] private int m_DebugPlayerCount = 1;

    public List<Player> PlayerList { private set; get; }

    public int NbPlayer => PlayerList.Count;

    public static GamePadsController Instance; // A static reference to the GameManager instance
    private void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        PlayerList = new List<Player>();

        var gamepads = Gamepad.all;
        // Debug.Log($"Number of gamepads: {gamepads.Count}");

        if (IsDebugMode)
        {
            m_DebugPlayerCount = Mathf.Clamp(m_DebugPlayerCount, 1, 4);

            for (int i = 0; i < m_DebugPlayerCount; i++)
            {
                InstantiateDebugPlayer(i);
            }
            return;
        }

        int index = 0;
        foreach (Gamepad gamepad in gamepads)
        {
            InstantiatePlayerUI("Gamepad", gamepad, index);
            index++;
        }
    }

    private void InstantiateDebugPlayer(int playerNumber)
    {
        var player = InstantiatePlayer(m_PlayerModels[playerNumber]);

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
    }

    private void InstantiatePlayerUI(string controlScheme, InputDevice device, int index)
    {
        var player = InstantiatePlayer(m_PlayerModels[index]);

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        if (GameManager.Instance.isInMainMenu && index == 0)
        {
            playerInput.SwitchCurrentActionMap("UI");
            player.gameObject.transform.position = new(-100, -100, -100);
        }
    }

    private Player InstantiatePlayer(PlayerModels model) 
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);

        // * Instantiate PlayerHeadFatigueBar UI
        if (!GameManager.Instance.isInMainMenu)
        {
            GameObject fatigueUIObj = Instantiate(m_HeadFatigueBarUI, m_MainCanvas.transform);
            PlayerHeadFatigueBar fatigueUI = fatigueUIObj.GetComponent<PlayerHeadFatigueBar>();
            fatigueUI.Initialize(player);
        }

        var a = Instantiate(model, player.transform);
        player.SetModelRef(a);

        //var renders = player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer r in renders)
        //{
        //    r.material = m_PlayerMAT[playerNumber];
        //}
        PlayerList.Add(player);

        return player;
    }
}
