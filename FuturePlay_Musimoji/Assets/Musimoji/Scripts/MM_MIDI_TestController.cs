using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Musimoji.Scripts;
using UnityEngine.Serialization;

public class MM_MIDI_TestController : MonoBehaviourPlus
{
    [SerializeField] private MinisNoteInputMapper inputMapper;
    [SerializeField] private MM_Midi_TestPlayer[] players;

    private void OnEnable()
    {
        inputMapper.OnNoteDown += SetPlayerEmoji;
    }

    private void OnDisable()
    {
        inputMapper.OnNoteDown -= SetPlayerEmoji;
    }

    private void SetPlayerEmoji(int playerId, Note note, float velocity)
    {
        if (players == null || players.Length <= playerId) return;
        players[playerId].SetEmoji(note, velocity);
    }
}

[Serializable]
public class EmojiNoteMap
{
    public NoteGroup_SO noteGroup;
    public Sprite emojiSprite;

    public bool Contains(Note note)
    {
        return noteGroup.Contains(note);
    }
}

[Serializable]
public enum Note
{
    C_Minus1 = 0,
    C_SharpMinus1 = 1,
    D_Minus1 = 2,
    D_SharpMinus1 = 3,
    E_Minus1 = 4,
    F_Minus1 = 5,
    F_SharpMinus1 = 6,
    G_Minus1 = 7,
    G_SharpMinus1 = 8,
    A_Minus1 = 9,
    A_SharpMinus1 = 10,
    B_Minus1 = 11,
    C0 = 12,
    C_Sharp0 = 13,
    D0 = 14,
    D_Sharp0 = 15,
    E0 = 16,
    F0 = 17,
    F_Sharp0 = 18,
    G0 = 19,
    G_Sharp0 = 20,
    A0 = 21,
    A_Sharp0 = 22,
    B0 = 23,
    C1 = 24,
    C_Sharp1 = 25,
    D1 = 26,
    D_Sharp1 = 27,
    E1 = 28,
    F1 = 29,
    F_Sharp1 = 30,
    G1 = 31,
    G_Sharp1 = 32,
    A1 = 33,
    A_Sharp1 = 34,
    B1 = 35,
    C2 = 36,
    C_Sharp2 = 37,
    D2 = 38,
    D_Sharp2 = 39,
    E2 = 40,
    F2 = 41,
    F_Sharp2 = 42,
    G2 = 43,
    G_Sharp2 = 44,
    A2 = 45,
    A_Sharp2 = 46,
    B2 = 47,
    C3 = 48,
    C_Sharp3 = 49,
    D3 = 50,
    D_Sharp3 = 51,
    E3 = 52,
    F3 = 53,
    F_Sharp3 = 54,
    G3 = 55,
    G_Sharp3 = 56,
    A3 = 57,
    A_Sharp3 = 58,
    B3 = 59,
    C4 = 60,
    C_Sharp4 = 61,
    D4 = 62,
    D_Sharp4 = 63,
    E4 = 64,
    F4 = 65,
    F_Sharp4 = 66,
    G4 = 67,
    G_Sharp4 = 68,
    A4 = 69,
    A_Sharp4 = 70,
    B4 = 71,
    C5 = 72,
    C_Sharp5 = 73,
    D5 = 74,
    D_Sharp5 = 75,
    E5 = 76,
    F5 = 77,
    F_Sharp5 = 78,
    G5 = 79,
    G_Sharp5 = 80,
    A5 = 81,
    A_Sharp5 = 82,
    B5 = 83,
    C6 = 84,
    C_Sharp6 = 85,
    D6 = 86,
    D_Sharp6 = 87,
    E6 = 88,
    F6 = 89,
    F_Sharp6 = 90,
    G6 = 91,
    G_Sharp6 = 92,
    A6 = 93,
    A_Sharp6 = 94,
    B6 = 95,
    C7 = 96,
    C_Sharp7 = 97,
    D7 = 98,
    D_Sharp7 = 99,
    E7 = 100,
    F7 = 101,
    F_Sharp7 = 102,
    G7 = 103,
    G_Sharp7 = 104,
    A7 = 105,
    A_Sharp7 = 106,
    B7 = 107,
    C8 = 108,
    C_Sharp8 = 109,
    D8 = 110,
    D_Sharp8 = 111,
    E8 = 112,
    F8 = 113,
    F_Sharp8 = 114,
    G8 = 115,
    G_Sharp8 = 116,
    A8 = 117,
    A_Sharp8 = 118,
    B8 = 119,
    C9 = 120,
    C_Sharp9 = 121,
    D9 = 122,
    D_Sharp9 = 123,
    E9 = 124,
    F9 = 125,
    F_Sharp9 = 126,
    G9 = 127
}
