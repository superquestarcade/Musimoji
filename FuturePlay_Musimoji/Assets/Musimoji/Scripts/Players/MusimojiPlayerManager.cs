using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusimojiPlayerManager : MonoBehaviourPlus
{
    public PlayerInputManager playerInputManager;
    public Transform[] playerTransforms;
    public SpriteRenderer[] emojiDisplays;
    public GameObject[] repressBeams;
    public GameObject playerControlPrefab;
    public MusimojiManager mmManager;
    
    private void AddPlayer(int playerIndex)
    {
        playerInputManager.JoinPlayer(playerIndex);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if(DebugMessages) Debug.Log($"OnPlayerJoined playerIndex {playerInput.playerIndex}");
        if (playerInput.playerIndex >= playerTransforms.Length) return;
        var mmPlayer = playerTransforms[playerInput.playerIndex].GetComponent<MusimojiPlayer>();
        var mmInput = playerInput.GetComponent<MusimojiInput>();
        mmInput.player = mmPlayer;
        var instance = playerInput.transform;
        instance.SetPositionAndRotation(playerTransforms[playerInput.playerIndex].transform.position, playerTransforms[playerInput.playerIndex].transform.rotation);
        instance.parent = playerTransforms[playerInput.playerIndex];
        mmPlayer.manager = mmManager;
        // mmPlayer.emojiDisplay = emojiDisplays[playerInput.playerIndex];
        // mmPlayer.repressBeam = repressBeams[playerInput.playerIndex];
        // mmPlayer.playerID = playerInput.playerIndex;
        mmPlayer.InitializeHuman();
    }
}
