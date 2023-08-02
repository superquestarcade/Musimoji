using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class MusimojiWinVFXManager : MonoBehaviour
{
    public bool debugMessages = false;

    public MusimojiManager manager;

    public Texture[] winEmojisSprites;

    public VisualEffect[] winEffects;
    
    public Vector2 winBurstMinMax = Vector2.up;

    [Range(0,1)]
    public float winEffectEachTrigger = 0.5f;

    private void OnEnable()
    {
        manager.OnStartGame.AddListener(StopWinEffects);
        manager.OnEndGame.AddListener(StopWinEffects);
        manager.OnWin.AddListener(StartWinEffects);
    }

    private void OnDisable()
    {
        manager.OnStartGame.RemoveListener(StopWinEffects);
        manager.OnEndGame.RemoveListener(StopWinEffects);
        manager.OnWin.RemoveListener(StartWinEffects);
    }

    private void StartWinEffects(int winId)
    {
        foreach(var effect in winEffects) effect.SetTexture("WinTexture", winEmojisSprites[winId-1]);
        
        StartCoroutine(WinEffect());
    }

    private IEnumerator WinEffect()
    {
        List<VisualEffect> effectList = new List<VisualEffect>(winEffects);
        
        int winEffectsToTrigger = Mathf.CeilToInt(winEffects.Length * winEffectEachTrigger);

        for (var we = 0; we < winEffectsToTrigger; we++)
        {
            var triggerEffect = effectList[Random.Range(0, effectList.Count)];

            triggerEffect.enabled = true;
            triggerEffect.Play();

            effectList.Remove(triggerEffect);
        }
        
        var waitTime = Random.Range(winBurstMinMax.x, winBurstMinMax.y);
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(WinEffect());
    }

    private void StopWinEffects()
    {
        StopAllCoroutines();
        foreach(var effect in winEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }
    }
}
