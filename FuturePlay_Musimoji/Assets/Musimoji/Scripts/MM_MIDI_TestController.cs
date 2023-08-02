using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Minis;
using UnityEngine.InputSystem.Utilities;

public class MM_MIDI_TestController : MonoBehaviourPlus
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Image emojiImageContainer;
    [SerializeField] private EmojiNoteMap[] emojiRefs;

    private int currentEmoji;

    private void Start()
    {
        InputSystem.onAnyButtonPress
            .CallOnce(OnAnyKeyMIDI);
    }

    public void OnMIDI_C(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started) return;
        Debug.Log("MM_MIDI_TestController.OnMIDI_C");
        currentEmoji++;
        if (currentEmoji >= emojiRefs.Length) currentEmoji = 0;
        SetEmoji(currentEmoji);
    }

    public void OnAnyKeyMIDI(InputControl inputControl)
    {
        if (!playerInput.devices.Contains(inputControl.device)) return;
        
    }

    private void SetEmoji(Note value)
    {
        if (!GetEmojiFromNote(value, out var s))
        {
            Debug.LogError($"MM_MIDI_TestController.SetEmoji no sprite found ({value})");
            return;
        }
        try
        {
            emojiImageContainer.sprite = s;
        }
        catch
        {
            Debug.LogError($"MM_MIDI_TestController.SetEmoji no sprite found ({value})");
        }
    }

    private bool GetEmojiFromNote(Note note, out Sprite sprite)
    {
        foreach (var emojiMap in emojiRefs)
        {
            if (emojiMap.notes.Contains(note))
            {
                sprite = emojiMap.emojiSprite;
                return true;
            }
        }

        sprite = null;
        return false;
    }

    public struct EmojiNoteMap
    {
        public Note[] notes;
        public Sprite emojiSprite;
    }
}

public enum Note
{
    C_Minus1,
    C_SharpMinus1,
    D_Minus1,
    D_SharpMinus1,
    E_Minus1,
    F_Minus1,
    F_SharpMinus1,
    G_Minus1,
    G_SharpMinus1,
    A_Minus1,
    A_SharpMinus1,
    B_Minus1,
    C0,
    C_Sharp0,
    D0,
    D_Sharp0,
    E0,
    F0,
    F_Sharp0,
    G0,
    G_Sharp0,
    A0,
    A_Sharp0,
    B0,
    C1,
    C_Sharp1,
    D1,
    D_Sharp1,
    E1,
    F1,
    F_Sharp1,
    G1,
    G_Sharp1,
    A1,
    A_Sharp1,
    B1,
    C2,
    C_Sharp2,
    D2,
    D_Sharp2,
    E2,
    F2,
    F_Sharp2,
    G2,
    G_Sharp2,
    A2,
    A_Sharp2,
    B2,
    C3,
    C_Sharp3,
    D3,
    D_Sharp3,
    E3,
    F3,
    F_Sharp3,
    G3,
    G_Sharp3,
    A3,
    A_Sharp3,
    B3,
    C4,
    C_Sharp4,
    D4,
    D_Sharp4,
    E4,
    F4,
    F_Sharp4,
    G4,
    G_Sharp4,
    A4,
    A_Sharp4,
    B4,
    C5,
    C_Sharp5,
    D5,
    D_Sharp5,
    E5,
    F5,
    F_Sharp5,
    G5,
    G_Sharp5,
    A5,
    A_Sharp5,
    B5,
    C6,
    C_Sharp6,
    D6,
    D_Sharp6,
    E6,
    F6,
    F_Sharp6,
    G6,
    G_Sharp6,
    A6,
    A_Sharp6,
    B6,
    C7,
    C_Sharp7,
    D7,
    D_Sharp7,
    E7,
    F7,
    F_Sharp7,
    G7,
    G_Sharp7,
    A7,
    A_Sharp7,
    B7,
    C8,
    C_Sharp8,
    D8,
    D_Sharp8,
    E8,
    F8,
    F_Sharp8,
    G8,
    G_Sharp8,
    A8,
    A_Sharp8,
    B8,
    C9,
    C_Sharp9,
    D9,
    D_Sharp9,
    E9,
    F9,
    F_Sharp9,
    G9
}
