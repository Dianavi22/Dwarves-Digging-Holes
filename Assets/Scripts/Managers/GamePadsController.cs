using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    public int nbPlayer;

    [Header("Player Instance")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] private List<Material> m_PlayerMAT = new();

    [Header("UI")]
    [SerializeField] private GameObject m_MainCanvas;
    [SerializeField] private GameObject[] m_UICanvas;
    [SerializeField] private GameObject m_HeadFatigueBarUI;

    [Header("Debug")]
    public bool IsDebugMode;
    [SerializeField, Range(1, 4)] private int m_DebugPlayerCount = 1;

    public List<Player> PlayerList { private set; get; }

    private Dictionary<Gamepad, PlayerInput> _playerGamepadMap = new();
    private Dictionary<Gamepad, Coroutine> _pendingDeviceAddRequests = new();

    public static GamePadsController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerList = new List<Player>();

        if (IsDebugMode)
        {
            m_DebugPlayerCount = Mathf.Clamp(m_DebugPlayerCount, 1, 4);
            for (int i = 0; i < m_DebugPlayerCount; i++)
            {
                InstantiateDebugPlayer(i);
            }
            return;
        }

        var gamepads = Gamepad.all;
        int index = 0;

        foreach (Gamepad gamepad in gamepads)
        {
            InstantiatePlayerUI("Gamepad", gamepad, index);
            index++;
            nbPlayer = index;
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

        var renders = player.GetComponentsInChildren<SkinnedMeshRenderer>();
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

        _playerGamepadMap.Add((Gamepad)device, playerInput);
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        GameObject fatigueUIObj = Instantiate(m_HeadFatigueBarUI, m_MainCanvas.transform);
        PlayerHeadFatigueBar fatigueUI = fatigueUIObj.GetComponent<PlayerHeadFatigueBar>();
        fatigueUI.Initialize(player);

        var renders = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer r in renders)
        {
            r.material = m_PlayerMAT[index];
        }

        PlayerList.Add(player);
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private HashSet<InputDevice> _recentlyProcessedDevices = new();
    private const float DeviceChangeCooldown = 0.5f; // Cooldown in seconds

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        //if (_recentlyProcessedDevices.Contains(device)) return;

        //StartCoroutine(ClearRecentlyProcessedDevice(device));

        if (device is not Gamepad gamepad) return;

        switch (change)
        {
            case InputDeviceChange.Added:
                HandleAddedDevice(gamepad);
                break;

            case InputDeviceChange.Reconnected:
                HandleReconnectedDevice(gamepad);
                break;

            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Removed:
                HandleDisconnectedDevice(gamepad);
                break;

            default:
                break;
        }
    }

    private IEnumerator ClearRecentlyProcessedDevice(InputDevice device)
    {
        _recentlyProcessedDevices.Add(device);
        yield return new WaitForSeconds(DeviceChangeCooldown);
        _recentlyProcessedDevices.Remove(device);
    }

    private void HandleAddedDevice(Gamepad gamepad)
    {
        if (_pendingDeviceAddRequests.ContainsKey(gamepad))
        {
            StopCoroutine(_pendingDeviceAddRequests[gamepad]);
            _pendingDeviceAddRequests.Remove(gamepad);
        }

        Coroutine coroutine = StartCoroutine(DelayedAddDevice(gamepad));
        _pendingDeviceAddRequests[gamepad] = coroutine;
    }

    private IEnumerator DelayedAddDevice(Gamepad gamepad)
    {
        yield return new WaitForSeconds(0.1f);

        // Vérifier si le gamepad est toujours valide et s'il n'a pas été géré entre-temps.
        if (!_playerGamepadMap.ContainsKey(gamepad))
        {
            KeyValuePair<Gamepad, PlayerInput> availableSlot = _playerGamepadMap.FirstOrDefault(kv => kv.Value.devices.Count == 0);

            if (availableSlot.Key != null && availableSlot.Value != null)
            {
                availableSlot.Value.SwitchCurrentControlScheme("Gamepad", gamepad);
                _playerGamepadMap.Remove(availableSlot.Key);
                _playerGamepadMap[gamepad] = availableSlot.Value;
                Debug.Log("Assigned new controller to an existing player.");
            }
            else
            {
                Debug.Log("No available player for the new controller.");
            }
        }

        // Supprimer la coroutine du dictionnaire après son exécution.
        _pendingDeviceAddRequests.Remove(gamepad);
    }

    private void HandleReconnectedDevice(Gamepad gamepad)
    {
        if (_playerGamepadMap.ContainsKey(gamepad))
        {
            PlayerInput playerInput = _playerGamepadMap[gamepad];
            if (!playerInput.devices.Contains(gamepad))
            {
                playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
                Debug.Log("Reconnected gamepad reassigned to the player.");
            }
        }
        else
        {
            HandleAddedDevice(gamepad);
        }
    }

    private void HandleDisconnectedDevice(Gamepad gamepad)
    {
        if (_playerGamepadMap.ContainsKey(gamepad))
        {
            _playerGamepadMap[gamepad].user.UnpairDevices();
            Debug.Log("Controller disconnected and unpaired from player.");
        }
    }
}
