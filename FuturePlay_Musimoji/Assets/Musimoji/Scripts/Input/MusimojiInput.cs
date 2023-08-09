using Musimoji;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusimojiInput : MonoBehaviourPlus
{
    public MusimojiPlayer player;
    
    #region Buttons

    public void OnButton1(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton1 started (player {player.playerID})");
            player.InitializeHuman();
            player.FireEmoji();
            player.ResetBotTimer();
        }
        
        if (callbackContext.canceled)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton1 canceled (player {player.playerID})");
        }
    }
    
    public void OnButton2(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton2 started (player {player.playerID})");
            player.InitializeHuman();
            player.NextEmoji();
            player.ResetBotTimer();
        }
        
        if (callbackContext.canceled)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton2 canceled (player {player.playerID})");
        }
    }
    
    public void OnButton3(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton3 started (player {player.playerID})");
            player.InitializeHuman();
            player.FireRepress();
            player.ResetBotTimer();
        }
        
        if (callbackContext.canceled)
        {
            if(DebugMessages) Debug.Log($"MusimojiInput.OnButton3 canceled (player {player.playerID})");
        }
    }

    #endregion
    
    #region MIDI Input

    public void OnMidiNoteDown(Note note, float velocity)
    {
        
    }

    public void OnMidiNoteUp(Note note)
    {
        
    }
    
    #endregion
}
