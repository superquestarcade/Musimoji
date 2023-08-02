using System;
using UnityEngine;

public class LEDSwitcher : MonoBehaviour
{
    private int currentState;
    private int stateCount;

    private void Start()
    {
        Debug.LogWarning($"LEDSwitcher.Start {currentState}");
        ArduinoLEDControl.SetState((LEDPlayState) currentState);
        stateCount = Enum.GetValues(typeof(LEDPlayState)).Length;
    }

    public void NextState()
    {
        currentState++;
        Debug.LogWarning($"LEDSwitcher.NextState {currentState}");
        if (currentState >= stateCount - 1) currentState = 0;
        ArduinoLEDControl.SetState((LEDPlayState) currentState);
    }

    public void SetState(int newState)
    {
        currentState = newState;
        ArduinoLEDControl.SetState((LEDPlayState) currentState);
    }
}
