using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;

    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject[] uiObjects;

    public bool debug;

    [SerializeField, Range(1, 4)] private int debugPlayerCount = 1;

    private int index = 0;

    [SerializeField] List<Material> _playerMAT = new();

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
        Player player = Instantiate(playerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(uiObjects[playerNumber], canvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        uiInfo.Initialize(player);
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
    }

    private void InstantiatePlayerUI(string controlScheme, InputDevice device)
    {
        Player player = Instantiate(playerPrefab, transform.parent);
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        GameObject ui = Instantiate(uiObjects[index], canvas.transform);
        PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();

        uiInfo.Initialize(player);
        playerInput.SwitchCurrentControlScheme(controlScheme, device);

        if(Utils.TryGetChildComponent<MeshRenderer>(player.gameObject, out var mat, 1))
        {
            mat.material = _playerMAT[index];
        }
        index++;
    }
}
