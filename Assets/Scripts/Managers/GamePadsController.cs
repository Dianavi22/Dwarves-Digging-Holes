using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [Header("Player Instance")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] List<Material> m_PlayerMAT = new();

    [Header("UI")]
    [SerializeField] private GameObject m_MainCanvas;
    [SerializeField] private GameObject[] m_UICanvas;

    [Header("Debug")]
    public bool IsDebugMode;
    [SerializeField, Range(1, 4)] private int m_DebugPlayerCount = 1;

    public List<Player> PlayerList { private set; get; }

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
    }

    void Start()
    {
        var gamepads = Gamepad.all;
        //Debug.Log($"Number of gamepads: {gamepads.Count}");

        if(IsDebugMode) {

            m_DebugPlayerCount = Mathf.Clamp(m_DebugPlayerCount, 1, 4);

            for(int i = 0; i < m_DebugPlayerCount; i++)
            {
                InstantiateDebugPlayer(i);
            }
            return;
        }

        int index = 0;
        foreach ( Gamepad gamepad in gamepads )
        {   
            InstantiatePlayerUI("Gamepad", gamepad, index);
            index++;
        }
    }

    private void InstantiateDebugPlayer(int playerNumber)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(m_UICanvas[playerNumber], m_MainCanvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        uiInfo.Initialize(player);
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
    }

    private void InstantiatePlayerUI(string controlScheme, InputDevice device, int index)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(m_UICanvas[index], m_MainCanvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        uiInfo.Initialize(player);
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        if(Utils.TryGetChildComponent<MeshRenderer>(player.gameObject, out var mat, 1))
        {
            mat.material = m_PlayerMAT[index];
        }

        PlayerList.Add(player);
    }
}
