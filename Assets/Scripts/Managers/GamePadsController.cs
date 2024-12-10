using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    public int nbPlayer;

    [Header("Player Instance")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] List<Material> m_PlayerMAT = new();

    [Header("UI")]
    [SerializeField] private GameObject m_MainCanvas;
    [SerializeField] private GameObject[] m_UICanvas;
    [SerializeField] private GameObject m_HeadFatigueBarUI;

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
        
        var gamepads = Gamepad.all;
        // Debug.Log($"Number of gamepads: {gamepads.Count}");

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
            nbPlayer = index;
        }
    }

    private void InstantiateDebugPlayer(int playerNumber)
    {
        Player player = Instantiate(m_PlayerPrefab, transform.position, transform.localRotation, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        // * Instantiate Player UI
        GameObject ui = Instantiate(m_UICanvas[playerNumber], m_MainCanvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();
        uiInfo.Initialize(player);

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
        Player player = Instantiate(m_PlayerPrefab, transform.position, transform.localRotation, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        // * Instantiate Player UI
        //GameObject ui = Instantiate(m_UICanvas[index], m_MainCanvas.transform);
        //PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();
        //uiInfo.Initialize(player);

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
    }
}
