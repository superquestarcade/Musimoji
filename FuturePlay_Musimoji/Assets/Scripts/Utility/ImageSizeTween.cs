using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageSizeTween : MonoBehaviourPlus
{
    [SerializeField] private RectTransform imageTransform;
    private Vector2 originalSize, targetSize, previousTargetSize;
    private float tweenTimer, tweenDuration;

    private Action onTweenComplete;
    
    // Start is called before the first frame update
    void Start()
    {
        previousTargetSize = originalSize = imageTransform.rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (tweenTimer <= 0)
        {
            return;
        }
        TweenImage();
    }

    private void TweenImage()
    {
        if (targetSize == originalSize) return;

        var t = 1-(tweenTimer / tweenDuration);
        imageTransform.sizeDelta = Vector2.Lerp(previousTargetSize, targetSize,t);
        if(DebugMessages) Debug.Log($"ImageSizeTween.TweenImage t {t}, size {imageTransform.sizeDelta}, " +
                                    $"previousSize {previousTargetSize}, targetSize {targetSize}");
        tweenTimer -= Time.deltaTime;
        if(tweenTimer <= 0) TweenComplete();
    }

    public void TweenStart(float size, float duration, Action callback = null)
    {
        if(DebugMessages) Debug.Log($"ImageSizeTween.TweenStart size {size}, duration {duration}");
        tweenDuration = tweenTimer = duration;
        previousTargetSize = targetSize;
        targetSize = originalSize * size;
        
        if(callback!=null) onTweenComplete += callback;
    }

    private void TweenComplete()
    {
        if(DebugMessages) Debug.Log("ImageSizeTween.TweenComplete");
        onTweenComplete?.Invoke();
        onTweenComplete = null;
    }
}
