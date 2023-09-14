using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_PlayerVfx : MonoBehaviourPlus
{
    [SerializeField] private MusimojiPlayer playerManager;
    [SerializeField] private ParticleSystem playParticleSystem, expressParticleSystem;
    [SerializeField] private ParticleSystem[] repressParticleSystems;

    [Header("Settings")] 
    [SerializeField] private Color[] emojiVfxColors;
    

    private void OnEnable()
    {
        playerManager.EmojiChangeEvent.AddListener(OnChangeEmoji);
        playerManager.EmojiExpressEvent.AddListener(OnExpress);
        playerManager.EmojiRepressEvent.AddListener(OnRepress);
    }

    private void OnRepress()
    {
        foreach (var repressParticleSystem in repressParticleSystems)
        {
            repressParticleSystem.Play();
        }
    }

    private void OnExpress()
    {
        expressParticleSystem.Play();
    }

    private void OnChangeEmoji(int emojiIndex)
    {
        SetColor(emojiIndex);
        playParticleSystem.Play();
    }

    private void SetColor(int emojiIndex)
    {
        var playMainModule = playParticleSystem.main;
        playMainModule.startColor = emojiVfxColors[emojiIndex];
        var expressMainModule = expressParticleSystem.main;
        expressMainModule.startColor = emojiVfxColors[emojiIndex];
        /*foreach (var repressParticleSystem in repressParticleSystems)
        {
            var repressMainModule = repressParticleSystem.main;
            repressMainModule.startColor = emojiVfxColors[emojiIndex];
        }*/
    }
}
