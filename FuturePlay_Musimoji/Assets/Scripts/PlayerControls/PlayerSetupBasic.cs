using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetupBasic : MonoBehaviour
{
    public bool debugMessages = false;

    public SpriteRenderer playerSprite;

    [SerializeField] private Color[] playerColors;

    public TMP_Text playerNumberText;

    [SerializeField] private int playerIndex;

    private void Start()
    {
        SetPlayerFromID();
    }

    private void SetPlayerFromID()
    {
        playerIndex = GetComponent<PlayerInput>().playerIndex;
        
        if (playerSprite != null && playerColors.Length>=playerIndex) playerSprite.color = playerColors[playerIndex];

        if (playerNumberText != null) playerNumberText.text = (playerIndex+1).ToString();
    }
}
