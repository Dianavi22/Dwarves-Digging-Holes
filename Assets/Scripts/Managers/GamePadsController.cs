using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject[] uiObjects;

    public bool debug;

    [SerializeField, Range(1, 4)] 
    private int debugPlayerCount = 1;

    private int index = 0;

    [SerializeField] List<Material> _playerMAT = new List<Material>();

    void Start()
    {
        var gamepads = Gamepad.all;
        //Debug.Log($"Number of gamepads: {gamepads.Count}");

        if(debug) {

           debugPlayerCount = Mathf.Clamp(debugPlayerCount, 1, 4);

            for(int i = 0; i < debugPlayerCount; i++)
            {
                InstantiateDebugPlayer(i);
            }

            return;
        }
        foreach ( Gamepad gamepad in gamepads )
        {   
            InstantiatePlayerUI("Gamepad", gamepad);
        }
    }

    private void InstantiateDebugPlayer(int playerNumber)
    {
        GameObject player = Instantiate(playerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(uiObjects[index], canvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        PlayerFatigue playerFatigue = player.GetComponent<PlayerFatigue>();
        uiInfo.Initialize(playerHealth, playerFatigue);

        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);

        index++;
    }

    private void InstantiatePlayerUI(string controlScheme, InputDevice device)
    {
        GameObject player = Instantiate(playerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(uiObjects[index], canvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        PlayerFatigue playerFatigue = player.GetComponent<PlayerFatigue>();
        uiInfo.Initialize(playerHealth, playerFatigue);
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        if(Utils.TryGetChildComponent<MeshRenderer>(player, out var mat, 1))
        {
            mat.material = _playerMAT[index];
        }
        index++;
    }

}
