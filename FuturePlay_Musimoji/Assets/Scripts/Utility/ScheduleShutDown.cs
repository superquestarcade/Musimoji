using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScheduleShutDown : MonoBehaviourPlus
{
    public bool shutDownAfterTime = false;
    public int hour, minutes, seconds;
    private DateTime shutDownTime;
    // Arduino LED Controller
    [SerializeField] private bool arduinoLedControlEnabled = true;

    private void Start()
    {
        shutDownTime = DateTime.Today + new TimeSpan(0, hour, minutes, seconds);
        Debug.LogWarning($"ScheduleShutDown shutDownTime is set to {shutDownTime}");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!shutDownAfterTime || DateTime.Now.TimeOfDay < shutDownTime.TimeOfDay) return;
        Debug.LogWarning("ScheduleShutDown shutting down based on scheduled time");
        // if(arduinoLedControlEnabled) ArduinoLEDControl.SetState(LEDPlayState.STANDBY);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

        shutDownAfterTime = false;
    }
}
