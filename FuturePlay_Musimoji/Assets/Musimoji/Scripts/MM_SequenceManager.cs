using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class MM_SequenceManager : MonoBehaviour
{
    public bool debugMessages = false, debugAudio = false;
    
    public MM_Sequencer sequencer;

    public int bpm = 120;

    // Start is called before the first frame update
    private void Start()
    {
        sequencer.SetBpm(bpm);

        sequencer.StartPlaying();
    }

    public void SetSequenceData(int[] sequenceData)
    {
        sequencer.SetSequence(sequenceData);
    }
}
