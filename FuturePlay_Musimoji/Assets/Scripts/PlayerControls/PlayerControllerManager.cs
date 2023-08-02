using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerControllerManager : MonoBehaviour
{
    public bool debugMessages = false;

    public PlayerInputManager playerInputManager;

    public InputActionAsset inputActionAsset;

    [SerializeField] private string[] actionMapNames;

    private List<PlayerInput> playerInputs = new List<PlayerInput>();
    
    // Start is called before the first frame update
    private void Start()
    {
        if (playerInputManager == null) playerInputManager = GetComponent<PlayerInputManager>();

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    #region Action Maps

    private void OnValidate()
    {
        actionMapNames = GetActionMapNames();
    }

    private string[] GetActionMapNames()
    {
        var names = new List<string>();
        foreach(var aMap in inputActionAsset.actionMaps) names.Add(aMap.name);
        return names.ToArray();
    }

    #endregion

    #region Players

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if(debugMessages) Debug.Log($"PlayerControllerManager.OnPlayerJoined {playerInput.playerIndex}");

        playerInputs.Add(playerInput);
        
        playerInput.SwitchCurrentActionMap(GetPlayerActionMapName(playerInput.playerIndex));
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        if(debugMessages) Debug.Log($"PlayerControllerManager.OnPlayerLeft {playerInput.playerIndex}");
        
        playerInputs.Remove(playerInput);
        
        Destroy(playerInput.gameObject);
    }

    private string GetPlayerActionMapName(int playerID)
    {
        var actionMap = actionMapNames[playerID + 1];
        if(debugMessages) Debug.Log($"PlayerControllerManager.GetPlayerActionMapName {actionMap}");
        return actionMap;
    }

    #endregion

    #region Device Monitor

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if(debugMessages) Debug.Log($"PlayerControllerManager.OnDeviceChange {device.name} ({change})");
        switch (change)
        {
            case InputDeviceChange.Added:
                // New Device.
                break;
            case InputDeviceChange.Disconnected:
                // Device got unplugged.
                break;
            case InputDeviceChange.Reconnected:
                // Plugged back in.
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                break;
            default:
                // See InputDeviceChange reference for other event types.
                break;
        }
    }

    #endregion
}
