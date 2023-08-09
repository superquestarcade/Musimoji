using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class MM_Sequencer : MonoBehaviour
{
    public bool debugMessages = false;
    
    private bool isPlaying = false;

    public bool playOnStart = true;

    public int bpm;
    [SerializeField] private int sequenceStepCount = 16;

    private int currentStep;
    public int SequenceStepCount => sequenceStepCount;
    
    public float StepDuration => (float) bpm/60/4;
    public float SequenceDuration => StepDuration * sequenceStepCount;

    [SerializeField] private float stepTimer, sequenceTimer;

    public float SequenceTimer => sequenceTimer;

    private int[] sequenceData;

    public int SequencerCount => sequenceData.Length;

    public UnityEvent<int, int> OnAnyStep;

    public UnityEvent<int[]> OnSetSequenceData;

    // Start is called before the first frame update
    private void Awake()
    {
        sequenceData = new int[sequenceStepCount];
    }

    private void FixedUpdate()
    {
        StepTick();
    }

    private void StepTick()
    {
        if (!isPlaying) return;
        
        stepTimer += Time.deltaTime;
        sequenceTimer += Time.deltaTime;

        if (stepTimer < StepDuration) return;
        
        if(debugMessages) Debug.Log("MM_Sequencer.StepTick");
        CalculateNextStep();
        OnAnyStep?.Invoke(currentStep, sequenceData[currentStep-1]);
    }

    private void CalculateNextStep()
    {
        currentStep++;
        stepTimer -= StepDuration;
        
        if (currentStep <= sequenceStepCount) return;
        
        currentStep -= sequenceStepCount;
        sequenceTimer -= SequenceDuration;
    }

    public void StartPlaying()
    {
        if (isPlaying) return;
        if(debugMessages) Debug.Log("MM_Sequencer.StartPlaying");
        isPlaying = true;
    }

    public void StopPlaying()
    {
        if(debugMessages) Debug.Log("MM_Sequencer.StopPlaying");
        isPlaying = false;
    }

    public void Restart()
    {
        if(debugMessages) Debug.Log("MM_Sequencer.Restart");
        StopPlaying();
        currentStep = 0;
        sequenceTimer = 0;
        StartPlaying();
    }

    public void SetBpm(int bpm)
    {
        if(debugMessages) Debug.Log($"MM_Sequencer.SetBpm {bpm}");
        this.bpm = bpm;
    }

    public void SetSequence(int[] sequence)
    {
        if(debugMessages) Debug.Log("MM_Sequencer.SetSequence");
        this.sequenceData = sequence;
        OnSetSequenceData?.Invoke(this.sequenceData);
    }

    public void SetStep(int step, int value)
    {
        if(debugMessages) Debug.Log($"MM_Sequencer.SetStep {step} value {value}");
        sequenceData[step] = value;
    }

    public float GetEmojiIntensity(int emojiIndex)
    {
        var returnIntensity = 0;

        foreach (var i in sequenceData)
        {
            if (i == emojiIndex) returnIntensity++;
            if (returnIntensity == 3) break;
        }

        return (float) returnIntensity/3;
    }
}
