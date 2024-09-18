using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadsController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    public bool debug;

    void Start()
    {
        var gamepads = Gamepad.all;
        //Debug.Log($"Number of gamepads: {gamepads.Count}");

        if(debug) {
            PlayerInput playerInput = Instantiate(playerPrefab, transform.parent).GetComponent<PlayerInput>();
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
            return;
        }
        foreach ( Gamepad gamepad in gamepads )
        {
            //Debug.Log(gamepad.displayName);
            PlayerInput playerInput = Instantiate(playerPrefab, transform.parent).GetComponent<PlayerInput>();
            playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);

        }
    }
}
