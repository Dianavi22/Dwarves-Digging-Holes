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

    private int index = 0;

    void Start()
    {
        var gamepads = Gamepad.all;
        //Debug.Log($"Number of gamepads: {gamepads.Count}");

        if(debug) {


            GameObject player = Instantiate(playerPrefab, transform.parent);
            PlayerInput playerInput = player.GetComponent<PlayerInput>();

            GameObject ui = Instantiate(uiObjects[index], canvas.transform);
            PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();
            uiInfo.playerHealth = player.GetComponent<PlayerHealth>();

            PlayerFatigue playerFatigue = player.GetComponent<PlayerFatigue>();
            uiInfo.playerFatigue = player.GetComponent<PlayerFatigue>();

            //uiInfo.Initialize(playerHealth, playerFatigue);
            
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);

            return;
        }
        foreach ( Gamepad gamepad in gamepads )
        {   
            //Debug.Log(gamepad.displayName);
            GameObject player = Instantiate(playerPrefab, transform.parent);
            PlayerInput playerInput = player.GetComponent<PlayerInput>();

            GameObject ui = Instantiate(uiObjects[index], canvas.transform);
            PlayerInformationManager uiInfo = ui.GetComponent<PlayerInformationManager>();
            uiInfo.playerHealth = player.GetComponent<PlayerHealth>();
            PlayerFatigue playerFatigue = player.GetComponent<PlayerFatigue>();

            //uiInfo.Initialize(playerHealth, playerFatigue);

            playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);

            index++;
        }
    }
}
