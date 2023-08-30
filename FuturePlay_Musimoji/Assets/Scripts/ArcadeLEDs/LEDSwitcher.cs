using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LEDSwitcher : MonoBehaviourPlus
{
    private int currentState;
    private LEDPlayState[] allStates;
    private int StateCount => allStates.Length;

    private void Start()
    {
        Debug.LogWarning($"LEDSwitcher.Start {currentState}");
        allStates = ArduinoLEDControl.GetAllPlayStates;
        ArduinoLEDControl.SetState(allStates[currentState]);
    }

    public void NextState()
    {
        currentState++;
        if (currentState >= StateCount - 1) currentState = 0;
        Debug.LogWarning($"LEDSwitcher.NextState {currentState}");
        ArduinoLEDControl.SetState(allStates[currentState]);
    }

    public void PreviousState()
    {
        currentState--;
        if (currentState < 0) currentState = StateCount-1;
        Debug.LogWarning($"LEDSwitcher.NextState {currentState}");
        ArduinoLEDControl.SetState(allStates[currentState]);
    }

    public void SetState(int newState)
    {
        currentState = newState;
        ArduinoLEDControl.SetState(allStates[currentState]);
    }
    
    #region Input

    public void OnNextState(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started) return;
        NextState();
    }

    public void OnPreviousState(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started) return;
        PreviousState();
    }
    #endregion
}
