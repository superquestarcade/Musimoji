using UnityEngine;
using UnityEngine.Events;

public class MM_Sequencer : MonoBehaviourPlus
{
    private bool isPlaying = false;
    public bool playOnStart = true;
    public int bpm;
    [SerializeField] private int sequenceStepCount = 16;
    private int currentStep;
    public int CurrentStepValue => CurrentSequence[currentStep];
    public int SequenceStepCount => sequenceStepCount;
    public float StepDuration => (float) bpm/60/4;
    public float SequenceDuration => StepDuration * sequenceStepCount;
    [SerializeField] private float stepTimer, sequenceTimer;
    public float SequenceTimer => sequenceTimer;
    public int[] CurrentSequence { get; private set; }

    public int SequencerCount => CurrentSequence.Length;
    public UnityEvent<int, int> OnAnyStep;
    public UnityEvent<int, int> OnSetStep;
    public UnityEvent<int[]> OnSetSequenceData;

    // Start is called before the first frame update
    private void Awake()
    {
        CurrentSequence = new int[sequenceStepCount];
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
        if(DebugMessages) Debug.Log("MM_Sequencer.StepTick");
        CalculateNextStep();
        OnAnyStep?.Invoke(currentStep, CurrentSequence[currentStep-1]);
    }

    private void CalculateNextStep()
    {
        currentStep++;
        stepTimer -= StepDuration;
        if (WrapStepCount(ref currentStep)) return;
        sequenceTimer -= SequenceDuration;
    }

    public bool WrapStepCount(ref int stepCount)
    {
        if (stepCount <= sequenceStepCount) return false;
        stepCount -= sequenceStepCount;
        return true;
    }

    public int StepsFromAtoB(int a, int b)
    {
        if (a < 0 || a > sequenceStepCount)
        {
            Debug.LogError($"MM_Sequencer.StepsFromAtoB value a out of sequence range ({a}/{sequenceStepCount})");
            return -1;
        }
        if (b < 0 || b > sequenceStepCount)
        {
            Debug.LogError($"MM_Sequencer.StepsFromAtoB value b out of sequence range ({b}/{sequenceStepCount})");
            return -1;
        }
        
        if (a <= b)
        {
            return b - a;
        }

        return (b + sequenceStepCount) - a;
    }

    public void StartPlaying()
    {
        if (isPlaying) return;
        if(DebugMessages) Debug.Log("MM_Sequencer.StartPlaying");
        isPlaying = true;
    }

    public void StopPlaying()
    {
        if(DebugMessages) Debug.Log("MM_Sequencer.StopPlaying");
        isPlaying = false;
    }

    public void Restart()
    {
        if(DebugMessages) Debug.Log("MM_Sequencer.Restart");
        StopPlaying();
        currentStep = 0;
        sequenceTimer = 0;
        StartPlaying();
    }

    public void SetBpm(int bpm)
    {
        if(DebugMessages) Debug.Log($"MM_Sequencer.SetBpm {bpm}");
        this.bpm = bpm;
    }

    public void SetSequence(int[] sequence)
    {
        if(DebugMessages) Debug.Log("MM_Sequencer.SetSequence");
        this.CurrentSequence = sequence;
        OnSetSequenceData?.Invoke(this.CurrentSequence);
    }

    public void SetStep(int step, int value)
    {
        if(DebugMessages) Debug.Log($"MM_Sequencer.SetStep {step} value {value}");
        CurrentSequence[step] = value;
        OnSetStep?.Invoke(step, value);
        OnSetSequenceData?.Invoke(CurrentSequence);
    }

    public float GetEmojiIntensity(int emojiIndex)
    {
        var returnIntensity = 0;

        foreach (var i in CurrentSequence)
        {
            if (i == emojiIndex) returnIntensity++;
            if (returnIntensity == 3) break;
        }

        return (float) returnIntensity/3;
    }
}
