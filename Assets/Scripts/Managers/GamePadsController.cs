using System.Collections;
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

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private List<KeyValuePair<InputDevice, InputDeviceChange>> inputDeviceChanges = new();
    private Tween handleDeviceChange = null;

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Only handle changes for Gamepad devices
        if (device is not Gamepad gamepad) return;

        Debug.Log($"Device Change: {change}");

        // Schedule the handling of device changes
        handleDeviceChange ??= DOVirtual.DelayedCall(0.5f, HandleDeviceChange);
        inputDeviceChanges.Add(new KeyValuePair<InputDevice, InputDeviceChange>(device, change));
    }

    private void HandleDeviceChange()
    {
        // Clear the scheduled handleDeviceChange reference
        handleDeviceChange = null;

        // Get the last device change event
        var lastChange = inputDeviceChanges.Last();
        inputDeviceChanges.Clear();

        // Gather all connected gamepads
        var allConnectedGamepads = new List<Gamepad>(Gamepad.all);
        var lostGamepads = new List<Gamepad>(allConnectedGamepads);

        // Check if the last changed device is still connected
        if (!allConnectedGamepads.Contains((Gamepad)lastChange.Key))
        {
            // Find the PlayerInput that lost its gamepad
            PlayerInput lostPlayerInput = FindLostPlayerInput(allConnectedGamepads, lostGamepads);

            // If there are lost gamepads, switch the control scheme for the lost PlayerInput
            if (lostGamepads.Any())
            {
                lostPlayerInput?.SwitchCurrentControlScheme("Gamepad", lostGamepads[0]);
            }
        }
    }

    private PlayerInput FindLostPlayerInput(List<Gamepad> allConnectedGamepads, List<Gamepad> lostGamepads)
    {
        PlayerInput lostPlayerInput = null;

        // Iterate through all PlayerInputs to find the one that lost its gamepad
        foreach (var playerInput in PlayerInput.all)
        {
            // If the PlayerInput has no devices, it has lost its gamepad
            if (playerInput.devices.Count == 0)
            {
                lostPlayerInput = playerInput;
                continue;
            }

            // Remove connected gamepads from the lostGamepads list
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

    private void InstantiateDebugPlayer(int playerNumber)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        // * Instantiate PlayerHeadFatigueBar UI
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

        // * Instantiate PlayerHeadFatigueBar UI
        GameObject fatigueUIObj = Instantiate(m_HeadFatigueBarUI, m_MainCanvas.transform);
        PlayerHeadFatigueBar fatigueUI = fatigueUIObj.GetComponent<PlayerHeadFatigueBar>();
        fatigueUI.Initialize(player);

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
    }
}
