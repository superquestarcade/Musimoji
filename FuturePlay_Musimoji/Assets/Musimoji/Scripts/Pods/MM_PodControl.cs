using System;
using UnityEngine;

public class MM_PodControl : MonoBehaviour
{
    [SerializeField] private MusimojiManager manager;
    [SerializeField] private PodIndicatorStepData[] indicators;

    private void OnEnable()
    {
        manager.sequencer.OnSetSequenceData.AddListener(UpdatePodsFromRelativeSequence);
    }

    private void OnDisable()
    {
        manager.sequencer.OnSetSequenceData.RemoveListener(UpdatePodsFromRelativeSequence);
    }

    private void UpdatePodsFromRelativeSequence(int[] sequencerData)
    {
        var relativeSequenceData = manager.GetPodSequenceValues(indicators.Length);
        for (var index = 0; index < relativeSequenceData.Length; index++)
        {
            var data = relativeSequenceData[index];
            indicators[index].display.SetEmoji(data);
        }
    }

    public void SetPodEmoji(int step, int value)
    {
        for (var index = 0; index < indicators.Length; index++)
        {
            var ind = indicators[index];
            if (ind.step != index) continue;
            ind.display.SetEmoji(value);
        }
    }

    public void ResetPods()
    {
        foreach(var ind in indicators) ind.display.SetEmoji(0);
    }

    [Serializable]
    public class PodIndicatorStepData
    {
        public int step;
        public MusimojiSequenceStepDisplay display;
    }
}
