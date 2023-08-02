using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusimojiBotInput : MonoBehaviourPlus
{
    #region Properties & Variables

    public bool canBeBot = true;
    public MusimojiPlayer player;
    [SerializeField] private bool isAwake = false;
    private BotState currentState = BotState.SELECTINGEMOJI;
    private int PlayerID => player.playerID;

    [Header("Settings")]
    // Selecting emoji
    [Range(0,1)]
    public float selectNewEmojiFactor = 0.5f;
    
    public float selectEmojiDelay = 0.5f;
    
    private float selectEmojiTimer = 0f;
    
    private int emojiTypeTarget = 0;

    // Firing
    [Range(0,1)]
    public float fireOrRepressFactor = 0.8f;
    
    public float fireDelay = 5f;

    private float fireTimer = 0f;
    
    // Repressing
    public float repressDelay = 5f;

    private float repressTimer = 0f;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        if (!canBeBot) return;
        player.OnSetHuman += SetAwake;
    }

    private void OnDisable()
    {
        if (!canBeBot) return;
        player.OnSetHuman -= SetAwake;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isAwake) return;
        
        PlayAsBot();
    }

    #endregion


    private void SetAwake(bool isHuman)
    {
        if(DebugMessages) Debug.Log($"MusimojiBotInput.SetAwake Player {PlayerID} {(isHuman?"is human":"is a bot")}");
        isAwake = !isHuman;

        if (!isAwake) return;

        SelectRandomEmojiTarget();
    }

    private void SelectRandomEmojiTarget()
    {
        emojiTypeTarget = Random.Range(1, player.emojiSprites.Length + 1);
        currentState = BotState.SELECTINGEMOJI;
    }

    private void PlayAsBot()
    {
        switch (currentState)
        {
            case BotState.SELECTINGEMOJI:
                if (selectEmojiTimer >= selectEmojiDelay)
                {
                    if (player.selectedEmoji != emojiTypeTarget)
                    {
                        if(DebugMessages) Debug.Log($"MusimojiBotInput.PlayAsBot Bot{PlayerID} selecting next emoji {player.selectedEmoji} => {emojiTypeTarget}");
                        player.NextEmoji();
                        selectEmojiTimer = 0;
                        break;
                    }
                    
                    if(DebugMessages) Debug.Log($"MusimojiBotInput.PlayAsBot Bot{PlayerID} found target emoji");

                    currentState = BotState.FIRING;
                }

                selectEmojiTimer += Time.deltaTime;
                break;
            case BotState.FIRING:
                if (fireTimer >= fireDelay)
                {
                    if (player.PredictWinningShot())
                    {
                        if(DebugMessages) Debug.LogWarning($"MusimojiBotInput.PlayAsBot Bot{PlayerID} waiting to avoid winning");
                    }
                    else
                    {
                        if(DebugMessages) Debug.Log($"MusimojiBotInput.PlayAsBot Bot{PlayerID} FIRE!");
                        player.FireEmoji();
                    }

                    fireTimer = 0f;

                    if (Random.Range(0, 1f) < selectNewEmojiFactor)
                    {
                        SelectRandomEmojiTarget();
                        return;
                    }
                    
                    currentState = Random.Range(0, 1f) < fireOrRepressFactor ? BotState.FIRING : BotState.REPRESSING;
                }

                fireTimer += Time.deltaTime;
                break;
            case BotState.REPRESSING:
                if (repressTimer >= repressDelay)
                {
                    if(DebugMessages) Debug.Log($"MusimojiBotInput.PlayAsBot Bot{PlayerID} REPRESS!");
                    player.FireRepress();

                    repressTimer = 0;
                    
                    if (Random.Range(0, 1f) < selectNewEmojiFactor)
                    {
                        SelectRandomEmojiTarget();
                        return;
                    }
                    
                    currentState = Random.Range(0, 1f) < fireOrRepressFactor ? BotState.FIRING : BotState.REPRESSING;
                }

                repressTimer += Time.deltaTime;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private enum BotState
    {
        SELECTINGEMOJI,
        FIRING,
        REPRESSING,
    }
}
