using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Musimoji;

public class MM_KeyNoteEmitter : MonoBehaviourPlus
{
    [SerializeField] private StudioEventEmitter eventEmitter;
    private PARAMETER_ID parameterID;
    [SerializeField] private NoteEmojiIndexReference[] noteEmojiRefs;

    private void Start()
    {
        if(DebugLevel>DebugMessageLevel.MINIMAL) Debug.Log($"MM_KeyNoteEmitter.Start parameter {eventEmitter.Params[0].Name}," +
                                                           $" value {eventEmitter.Params[0].Value}");
        parameterID = eventEmitter.Params[0].ID;
        eventEmitter.Play();
    }

    public void OnSetNote(int emojiIndex)
    {
        var noteIndex = GetNoteIndexFromEmoji(emojiIndex);
        Debug.Assert(noteIndex>=0);
        eventEmitter.EventInstance.setParameterByID(parameterID, noteIndex, true);
        eventEmitter.EventInstance.start();
        if (DebugLevel > DebugMessageLevel.MINIMAL)
        {
            eventEmitter.EventInstance.getParameterByID(parameterID, out var paramValue);
            Debug.Log($"MM_KeyNoteEmitter.OnSetNote emojiIndex {emojiIndex}, noteIndex {noteIndex}, " +
                      $"parameter value {paramValue}");
        }
    }
    
    private int GetNoteIndexFromEmoji(int emojiIndex)
    {
        foreach (var noteEmojiRef in noteEmojiRefs)
        {
            if (noteEmojiRef.emojiIndex != emojiIndex) continue;
            if(DebugLevel>DebugMessageLevel.MINIMAL) Debug.Log($"MM_KeyNoteEmitter.GetNoteIndexFromEmoji emojiIndex {emojiIndex}, " +
                                                               $"noteIndex {noteEmojiRef.noteIndex}");
            return noteEmojiRef.noteIndex;
        }
        if(DebugLevel>DebugMessageLevel.MINIMAL) Debug.LogError($"MM_KeyNoteEmitter.GetNoteIndexFromEmoji " +
                                                                $"could not find note reference for emojiIndex {emojiIndex}");
        return -1;
    }
    
    #region Data
    
    // Note parameters
    // 0 = C
    // 1 = D
    // 2 = E
    // 3 = F
    // 4 = G
    // 5 = A
    // 6 = B
    // 7 = C2

    [Serializable]
    private struct NoteEmojiIndexReference
    {
        public int emojiIndex;
        public int noteIndex;
    }
    #endregion
}
