using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SceneLoadProgressBar : MonoBehaviour
{
    public SceneLoadManager sceneLoadManager;

    public GameObject loadScreen;

    private Image[] loadScreenImages;
    
    public Image loadBarBgImage, loadBarForeImage;
    
    public float fadeTime = 3;

    private void Start()
    {
        loadScreenImages = loadScreen.GetComponentsInChildren<Image>();
    }

    private void OnEnable()
    {
        sceneLoadManager.OnStartSceneLoad += StartLoading;
        sceneLoadManager.OnSceneLoadProgress += UpdateLoadBar;
        sceneLoadManager.OnEndSceneLoad += EndLoading;
    }

    private void OnDisable()
    {
        sceneLoadManager.OnStartSceneLoad -= StartLoading;
        sceneLoadManager.OnSceneLoadProgress -= UpdateLoadBar;
        sceneLoadManager.OnEndSceneLoad -= EndLoading;
    }

    private void StartLoading()
    {
        loadScreen.SetActive(true);
        loadBarBgImage.gameObject.SetActive(true);
        SetLoadScreenAlpha(1);
    }

    private void EndLoading(bool mainMenu = false)
    {
        StartCoroutine(mainMenu ? FadeIn() : FadeOut());
    }

    private void UpdateLoadBar(float progress)
    {
        loadBarForeImage.rectTransform.localScale = new Vector3(Mathf.Clamp(progress, 0, 1), 1);
    }

    private IEnumerator FadeOut()
    {
        var fadeTimer = fadeTime;
        while (fadeTimer > 0)
        {
            SetLoadScreenAlpha(Mathf.Clamp(fadeTimer/fadeTime, 0, 1));
            fadeTimer -= Time.deltaTime;
            yield return null;
        }
        loadBarBgImage.gameObject.SetActive(false);
        loadScreen.SetActive(false);
        
        //This is a workaround to reenable the InputSystemUIInputModule which stops responding on unloading scenes
        var eSystem = EventSystem.current.gameObject;
        eSystem.SetActive(false);
        eSystem.SetActive(true);
    }

    private IEnumerator FadeIn()
    {
        var fadeTimer = fadeTime;
        while (fadeTimer > 0)
        {
            SetLoadScreenAlpha(Mathf.Clamp(1-fadeTimer/fadeTime, 0, 1));
            fadeTimer -= Time.deltaTime;
            yield return null;
        }
        loadBarBgImage.gameObject.SetActive(false);
        loadScreen.SetActive(true);
        
        //This is a workaround to reenable the InputSystemUIInputModule which stops responding on unloading scenes
        var eSystem = EventSystem.current.gameObject;
        eSystem.SetActive(false);
        eSystem.SetActive(true);
    }

    private void SetLoadScreenAlpha(float value)
    {
        if(loadScreenImages==null) return;
        
        foreach(var i in loadScreenImages) SetImageAlpha(i, value);
        SetImageAlpha(loadBarBgImage, value);
        SetImageAlpha(loadBarForeImage, value);
    }

    private void SetImageAlpha(Image image, float value)
    {
        var imageColor = image.color;
        image.color = new Color(imageColor.r, imageColor.g, imageColor.b, value);
    }
}
