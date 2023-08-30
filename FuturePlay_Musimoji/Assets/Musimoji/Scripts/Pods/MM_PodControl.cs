using System;
using UnityEngine;

public class MM_PodControl : MonoBehaviourPlus
{
    [SerializeField] private MusimojiManager manager;
    [SerializeField] private MM_Sequencer sequencer;
    [SerializeField] private PodIndicatorStepData[] indicators;

    [Header("Settings")] 
    [SerializeField] private bool indicatorsEnabled = false;
    [SerializeField] private bool podsEnabled = true;

    private void OnEnable()
    {
        manager.sequencer.OnAnyStep.AddListener(OnStep);
        SetIndicatorsEnabled(indicatorsEnabled);
    }

    private void OnDisable()
    {
        manager.sequencer.OnAnyStep.RemoveListener(OnStep);
    }

    private void OnStep(int step, int stepValue)
    {
        if (indicatorsEnabled) UpdateIndicatorsFromRelativeSequence();
    }

    private void UpdateIndicatorsFromRelativeSequence()
    {
        if(DebugLevel>DebugMessageLevel.MINIMAL) Debug.Log("MM_PodControl.UpdatePodsFromRelativeSequence");
        for (var i = 0; i < indicators.Length; i++)
        {
            var indicator = indicators[i];
            var podStepValue = manager.GetPodStepValue(indicator.step);
            if(DebugLevel>DebugMessageLevel.MINIMAL) Debug.Log("MM_PodControl.UpdatePodsFromRelativeSequence" +
                                                               $"indicator {i}, step {indicator.step}, value {podStepValue}");
            indicator.display.SetEmoji(podStepValue);
        }
    }

    public void SetPodEmoji(int step, int value)
    {
        if (!indicatorsEnabled) return;
        for (var index = 0; index < indicators.Length; index++)
        {
            var ind = indicators[index];
            if (ind.step != index) continue;
            ind.display.SetEmoji(value);
        }
    }

    public void ResetPods()
    {
        if (!indicatorsEnabled) return;
        foreach(var ind in indicators) ind.display.SetEmoji(0);
    }

    private void SetIndicatorsEnabled(bool value)
    {
        foreach(var ind in indicators) ind.display.gameObject.SetActive(value);
    }

    [Serializable]
    public class PodIndicatorStepData
    {
        public int step;
        public MusimojiSequenceStepDisplay display;
    }
}
