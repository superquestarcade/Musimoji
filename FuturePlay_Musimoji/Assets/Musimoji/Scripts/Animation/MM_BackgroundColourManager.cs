using System;
using UnityEngine;

public class MM_BackgroundColourManager : MonoBehaviourPlus
{
    [SerializeField] private MusimojiManager mmManager;
    [SerializeField] private MM_Sequencer mmSequencer;
    [SerializeField] private Color[] emojiColorsPlaySpace = Array.Empty<Color>(), emojiColorsBorderSpace = Array.Empty<Color>();
    [SerializeField] private GameObject playspaceParent, borderParent;
    [SerializeField] private SpriteRenderer[] triSpriteRenderersPlaySpace, triSpriteRenderersBorderSpace;
    private System.Random rng = new System.Random();

    [Header("Debugging")] 
    [SerializeField] private bool populateArrays = false;

    private void OnValidate()
    {
        if (!populateArrays) return;
        triSpriteRenderersPlaySpace = playspaceParent.GetComponentsInChildren<SpriteRenderer>();
        triSpriteRenderersBorderSpace = borderParent.GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        ClearColors();
    }

    private void OnEnable()
    {
        mmManager.sequencer.OnSetSequenceData.AddListener(OnSetSequenceData);
        mmManager.sequencer.OnSetStep.AddListener(OnSetAnyStep);
    }

    private void OnDisable()
    {
        mmManager.sequencer.OnSetSequenceData.RemoveListener(OnSetSequenceData);
        mmManager.sequencer.OnSetStep.RemoveListener(OnSetAnyStep);
    }

    private void OnSetAnyStep(int step, int stepValue)
    {
        if(DebugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetAnyStep step {step}, value {stepValue}");
        OnSetPlayspaceTriColours(step, stepValue);
    }

    private void OnSetSequenceData(int[] sequenceData)
    {
        if(DebugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetSequenceData {sequenceData.Length}");
        OnSetBorderTriColours(sequenceData);
    }
    
    private void OnSetPlayspaceTriColours(int step, int stepValue)
    {
        if(DebugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetPlayspaceTriColours " +
                                    $"for {triSpriteRenderersPlaySpace.Length} tris");
        foreach (var tri in triSpriteRenderersPlaySpace)
        {
            var sampleSequenceValue = SampleSequence(mmSequencer.CurrentSequence, (float) rng.NextDouble());
            Debug.Assert(sampleSequenceValue<emojiColorsPlaySpace.Length, 
                $"MM_BackgroundColourManager.OnSetPlayspaceTriColours sampleSequenceValue {sampleSequenceValue}/{emojiColorsPlaySpace.Length}");
            var setColor = stepValue == sampleSequenceValue ? emojiColorsPlaySpace[sampleSequenceValue]:Color.clear;
            tri.color = setColor;
        }
    }

    private void OnSetBorderTriColours(int[] sequenceData)
    {
        if(DebugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetBorderTriColours " +
                                    $"for {triSpriteRenderersBorderSpace.Length} tris");
        foreach (var tri in triSpriteRenderersBorderSpace)
            tri.color = emojiColorsBorderSpace[SampleSequence(sequenceData, (float) rng.NextDouble())];
    }

    private int SampleSequence(int[] sequence, float t)
    {
        Debug.Assert(t is <= 1 and >= 0, $"MM_BackgroundColourManager.SampleSequence input invalid ({t}) should be 0 - 1");
        var sampleIndex = Mathf.RoundToInt((sequence.Length - 1) * t);
        return sequence[sampleIndex];
    }

    private void ClearColors()
    {
        if (DebugMessages) Debug.Log("MM_BackgroundColourManager.ClearColors");
        foreach (var tri in triSpriteRenderersBorderSpace)
            tri.color = Color.clear;
        foreach (var tri in triSpriteRenderersPlaySpace)
            tri.color = Color.clear;
    }
}
