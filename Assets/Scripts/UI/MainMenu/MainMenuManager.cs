using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _closeButtonSettings;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void LoadCreditScene()
    {

    }

    public void LoadSettingScene()
    {
        _settingsWindow.SetActive(true);
        _eventSystem.firstSelectedGameObject = _closeButtonSettings;
    }

    private HashSet<InputDevice> connectedDevices = new HashSet<InputDevice>();
    private Coroutine pendingConnections;

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
        switch (change)
        {
            case InputDeviceChange.Added:
                if (!connectedDevices.Contains(device))
                {
                    if (pendingConnections != null)
                    {
                        StopCoroutine(pendingConnections);
                    }

                    StartConnectionProcess(device);
                }
                break;

            case InputDeviceChange.Removed:
                if (connectedDevices.Contains(device))
                {
                    Destroy(PlayerInput.all.FirstOrDefault(pi => pi.devices.Contains(device)).gameObject);
                    connectedDevices.Remove(device);
                }
                break;

            default:
                break;
        }
    }

    private void StartConnectionProcess(InputDevice device)
    {
        pendingConnections = StartCoroutine(DelayedConnection(device));
    }

    private IEnumerator DelayedConnection(InputDevice device)
    {
        yield return new WaitForSeconds(0.75f); // Adjust the delay as needed

        if (!connectedDevices.Contains(device))
        {
            connectedDevices.Add(device);
            Debug.Log($"Controller connected: {device.displayName}");
            GamePadsController.Instance.AddNewPlayer(device);
            foreach (PlayerInput item in PlayerInput.all)
            {
                Debug.Log(item.currentControlScheme);
                if(!item.devices.Any() || item.currentControlScheme == "Player1") {
                    Destroy(item.gameObject);
                }
            }
        }

        if (pendingConnections != null)
        {
            pendingConnections = null;
        }
    }

}
