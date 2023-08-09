using Musimoji;
using UnityEngine;

public class MM_MIDI_PlayerManager : MonoBehaviourPlus
{
    [SerializeField] private MinisNoteInputMapper inputMapper;
    [SerializeField] private MM_MidiPlayerInput[] players;

    private void OnEnable()
    {
        inputMapper.OnNoteDown += OnPlayerNoteDown;
        inputMapper.OnNoteUp += OnPlayerNoteUp;
    }

    private void OnDisable()
    {
        inputMapper.OnNoteDown -= OnPlayerNoteDown;
        inputMapper.OnNoteUp -= OnPlayerNoteUp;
    }

    private void OnPlayerNoteDown(int playerId, Note note, float velocity)
    {
        if (players == null || players.Length <= playerId) return;
        players[playerId].OnMidiNoteDown(note, velocity);
    }
    
    private void OnPlayerNoteUp(int playerId, Note note)
    {
        if (players == null || players.Length <= playerId) return;
        players[playerId].OnMidiNoteUp(note);
    }
}
