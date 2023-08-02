using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_PlayerAnimationManager : MonoBehaviour
{
    public bool debugMessages = false;

    [SerializeField] private MusimojiPlayer player;
    
    [SerializeField] private Animator playerAnimator, botAnimator;
    
    private Animator Anim => (player.IsHuman ? playerAnimator : botAnimator);

    private void OnEnable()
    {
        player.OnSetHuman += OnSetHuman;
    }

    private void OnDisable()
    {
        player.OnSetHuman -= OnSetHuman;
    }

    private void OnSetHuman(bool isHuman)
    {
        playerAnimator.gameObject.SetActive(isHuman);
        botAnimator.gameObject.SetActive(!isHuman);
    }

    public void OnExpress()
    {
        Anim.SetTrigger("Express");
    }

    public void OnRepress()
    {
        Anim.SetTrigger("Repress");
    }

    public void OnChangeEmoji()
    {
        
    }

    public void OnReload()
    {
        
    }

    public void OnHit()
    {
        
    }
}
