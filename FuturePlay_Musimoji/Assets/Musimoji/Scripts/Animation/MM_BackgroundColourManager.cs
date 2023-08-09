using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MM_BackgroundColourManager : MonoBehaviour
{
    public bool debugMessages = false;
    
    [SerializeField] private MusimojiManager mmManager;
    
    [SerializeField] private Color[] emojiColorsPlaySpace = Array.Empty<Color>(), emojiColorsBorderSpace = Array.Empty<Color>();

    [SerializeField] private SpriteRenderer[] triSpriteRenderersPlaySpace, triSpriteRenderersBorderSpace;

    private System.Random rng = new System.Random();

    private void OnEnable()
    {
        mmManager.sequencer.OnSetSequenceData.AddListener(OnSetSequenceData);
    }

    private void OnDisable()
    {
        mmManager.sequencer.OnSetSequenceData.RemoveListener(OnSetSequenceData);
    }

    private void OnSetSequenceData(int[] sequenceData)
    {
        if(debugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetSequenceData {sequenceData.Length}");

        OnSetTriColours(sequenceData);
    }

    private void OnSetTriColours(int[] sequenceData)
    {
        if(debugMessages) Debug.Log($"MM_BackgroundColourManager.OnSetTriColours for {triSpriteRenderersPlaySpace.Length * triSpriteRenderersBorderSpace.Length} tris");

        foreach (var tri in triSpriteRenderersPlaySpace)
            tri.color = emojiColorsPlaySpace[SampleSequence(sequenceData, (float) rng.NextDouble())];
        
        foreach (var tri in triSpriteRenderersBorderSpace)
            tri.color = emojiColorsBorderSpace[SampleSequence(sequenceData, (float) rng.NextDouble())];
    }

    private int SampleSequence(int[] sequence, float t)
    {
        Debug.Assert(t is <= 1 and >= 0, $"MM_BackgroundColourManager.SampleSequence input invalid ({t}) should be 0 - 1");
        var sampleIndex = Mathf.RoundToInt((sequence.Length - 1) * t);
        return sequence[sampleIndex];
    }
}
