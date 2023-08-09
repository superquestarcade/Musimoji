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

    [SerializeField] private bool introActive = false;
    private float introTimer;

    private int activeSlideIndex = 0;
    [SerializeField] private int currentSlide = 0;

    private void Start()
    {
        if (!introActive) SlideShowComplete();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!introActive) return;
        introTimer += Time.deltaTime;
        if(introSlides == null) SlideShowComplete();
        if(introTimer >= slideDuration * introSlides.Length) SlideShowComplete();
        currentSlide = Mathf.FloorToInt(Mathf.Lerp(0,introSlides.Length, introTimer / (introSlides.Length * slideDuration)));
        if(activeSlideIndex!=currentSlide) SetActiveSlide(currentSlide);
    }
    
    public void StartIntro()
    {
        if(DebugMessages) Debug.Log("MM_Intro.StartIntro");
        introActive = true;
        SetActiveSlide(0);
        introCanvas.gameObject.SetActive(true);
    }

    private void SetActiveSlide(int slideIndex)
    {
        if(DebugMessages) Debug.Log($"MM_Intro.SetActiveSlide setting active slide {slideIndex}");
        for(var s=0;s<introSlides.Length;s++) introSlides[s].gameObject.SetActive(s==slideIndex);
        activeSlideIndex = slideIndex;
    }

    private void SlideShowComplete()
    {
        if(DebugMessages) Debug.Log("MM_Intro.SlideShowComplete");
        introActive = false;
        introTimer = 0;
        introCanvas.gameObject.SetActive(false);
        OnStartGame?.Invoke();
    }
}
