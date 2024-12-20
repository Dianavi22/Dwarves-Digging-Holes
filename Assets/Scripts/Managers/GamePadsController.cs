using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [Header("Player Instance")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] List<Material> m_PlayerMAT = new();

    [Header("UI")]
    [SerializeField] private GameObject m_MainCanvas;
    [SerializeField] private GameObject m_HeadFatigueBarUI;

    [Header("Debug")]
    public bool IsDebugMode;
    [SerializeField, Range(1, 4)] private int m_DebugPlayerCount = 1;

    public List<Player> PlayerList { private set; get; } = new();
    public int NbPlayer => PlayerList.Count;

    public static GamePadsController Instance;

    private Dictionary<PlayerInput, Gamepad> playerGamepadMap = new();
    private List<KeyValuePair<InputDevice, InputDeviceChange>> inputDeviceChanges = new();
    private Tween handleDeviceChange = null;

    private int index = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (IsDebugMode)
        {
            m_DebugPlayerCount = Mathf.Clamp(m_DebugPlayerCount, 1, 4);
            for (int i = 0; i < m_DebugPlayerCount; i++)
            {
                InstantiateDebugPlayer(i);
            }
            return;
        }

        InitializePlayers();
    }

    private void InitializePlayers()
    {
        var gamepads = Gamepad.all;
        foreach (Gamepad gamepad in gamepads)
        {
            InstantiatePlayerUI("Gamepad", gamepad, index);
            index++;
        }
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad gamepad) return;

        Debug.Log($"Device Change: {change}");
        handleDeviceChange ??= DOVirtual.DelayedCall(0.5f, HandleDeviceChange);
        inputDeviceChanges.Add(new KeyValuePair<InputDevice, InputDeviceChange>(device, change));
    }

    private void HandleDeviceChange()
    {
        handleDeviceChange = null;
        var lastChange = inputDeviceChanges.Last();
        inputDeviceChanges.Clear();

        var allConnectedGamepads = new List<Gamepad>(Gamepad.all);
        var lostGamepads = new List<Gamepad>(allConnectedGamepads);

        if (!allConnectedGamepads.Contains((Gamepad)lastChange.Key))
        {
            PlayerInput lostPlayerInput = FindLostPlayerInput(allConnectedGamepads, lostGamepads);
            if (lostGamepads.Any())
            {
                // Reassign the first available gamepad to the lost player input
                if (lostPlayerInput == null)
                {
                    if (index >= 3 || !GameManager.Instance.isInMainMenu) return;
                    InstantiatePlayerUI("Gamepad", lostGamepads[0], index);
                    Player added = PlayerList.Last();
                    added.GetMovement().SetStats(GameManager.Instance.Difficulty.PlayerStats);
                    added.GetFatigue().DefineStats(GameManager.Instance.Difficulty.MiningFatigue, GameManager.Instance.Difficulty.PushCartFatigue);
                    index++;
                }
                else
                {
                    lostPlayerInput?.SwitchCurrentControlScheme("Gamepad", lostGamepads[0]);
                    playerGamepadMap[lostPlayerInput] = lostGamepads[0];
                }

            }
        }
        else if (lastChange.Value == InputDeviceChange.Added)
        {
            // If a new gamepad is added, assign it to the next available player
            AssignNewGamepadToPlayer((Gamepad)lastChange.Key);
        }
    }

    private PlayerInput FindLostPlayerInput(List<Gamepad> allConnectedGamepads, List<Gamepad> lostGamepads)
    {
        PlayerInput lostPlayerInput = null;

        foreach (var playerInput in PlayerInput.all)
        {
            if (playerInput.devices.Count == 0)
            {
                lostPlayerInput = playerInput;
                continue;
            }

            foreach (var connectedGamepad in allConnectedGamepads)
            {
                if (playerInput.devices.Contains(connectedGamepad))
                {
                    lostGamepads.Remove(connectedGamepad);
                }
            }
        }

        return lostPlayerInput;
    }

    private void AssignNewGamepadToPlayer(Gamepad gamepad)
    {
        // Find the first available PlayerInput that is not currently assigned a gamepad
        foreach (var playerInput in PlayerInput.all)
        {
            if (!playerGamepadMap.ContainsKey(playerInput))
            {
                playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
                playerGamepadMap[playerInput] = gamepad;
                Debug.Log($"Assigned Gamepad {gamepad.displayName} to PlayerInput {playerInput}");
                break;
            }
        }
    }

    private void InstantiateDebugPlayer(int playerNumber)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject fatigueUIObj = Instantiate(m_HeadFatigueBarUI, m_MainCanvas.transform);
        PlayerHeadFatigueBar fatigueUI = fatigueUIObj.GetComponent<PlayerHeadFatigueBar>();
        fatigueUI.Initialize(player);

        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);

        var renders = player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer r in renders)
        {
            r.material = m_PlayerMAT[playerNumber];
        }
        PlayerList.Add(player);
    }

    private void InstantiatePlayerUI(string controlScheme, InputDevice device, int index)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        if (!GameManager.Instance.isInMainMenu)
        {
            GameObject fatigueUIObj = Instantiate(m_HeadFatigueBarUI, m_MainCanvas.transform);
            PlayerHeadFatigueBar fatigueUI = fatigueUIObj.GetComponent<PlayerHeadFatigueBar>();
            fatigueUI.Initialize(player);
        }


        var renders = player.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer r in renders)
        {
            r.material = m_PlayerMAT[index];
        }

        PlayerList.Add(player);
        if (GameManager.Instance.isInMainMenu && index == 0)
        {
            playerInput.SwitchCurrentActionMap("UI");
            player.gameObject.transform.position = new(-100, -100, -100);
        }

        // Map the player input to the gamepad
        playerGamepadMap[playerInput] = (Gamepad)device;
    }
}