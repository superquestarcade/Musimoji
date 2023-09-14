using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MM_Intro : MonoBehaviourPlus
{
    public Canvas introCanvas;
    public Image[] introSlides;
    public float slideDuration = 3f;
    public UnityEvent OnStartGame;

    [SerializeField] protected bool introActive = false;
    protected float introTimer;

    protected int activeSlideIndex = 0;
    [SerializeField] protected int currentSlide = 0;

    protected virtual void Start()
    {
        if (!introActive) SlideShowComplete();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!introActive) return;
        introTimer += Time.deltaTime;
        if(introSlides == null) SlideShowComplete();
        if(introTimer >= slideDuration * introSlides.Length) SlideShowComplete();
        currentSlide = Mathf.FloorToInt(Mathf.Lerp(0,introSlides.Length, introTimer / (introSlides.Length * slideDuration)));
        if(activeSlideIndex!=currentSlide) SetActiveSlide(currentSlide);
    }
    
    public virtual void StartIntro()
    {
        if(DebugMessages) Debug.Log("MM_Intro.StartIntro");
        introActive = true;
        SetActiveSlide(0);
        introCanvas.gameObject.SetActive(true);
    }

    protected virtual void SetActiveSlide(int slideIndex)
    {
        if(DebugMessages) Debug.Log($"MM_Intro.SetActiveSlide setting active slide {slideIndex}");
        for(var s=0;s<introSlides.Length;s++) introSlides[s].gameObject.SetActive(s==slideIndex);
        activeSlideIndex = slideIndex;
    }

    protected virtual void SlideShowComplete()
    {
        if(DebugMessages) Debug.Log("MM_Intro.SlideShowComplete");
        introActive = false;
        introTimer = 0;
        introCanvas.gameObject.SetActive(false);
        OnStartGame?.Invoke();
    }
}
