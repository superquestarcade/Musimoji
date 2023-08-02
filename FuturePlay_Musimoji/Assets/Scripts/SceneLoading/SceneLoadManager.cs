using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneLoadManager : MonoBehaviour
{
    public bool debugMessages = false;
    public static SceneLoadManager singleton { get; private set; }
    public bool dontDestroyOnLoad = true;
    public string mainMenuScene;
    public string[] startScenes;
    private string mainMenuSceneName;
    private string[] startSceneNames;
    private bool isLoading = false;
    private string currentSceneName;
    [SerializeField] private Button[] gameLoadButtons;
    private AudioSource menuAudio;
    private AsyncOperation sceneToLoad;
    private List<Scene> activeScenes = new List<Scene>();
    // Arduino LED Control
    [SerializeField] private bool arduinoLedControlEnabled = true;

    public Action OnStartSceneLoad;
    public Action<bool> OnEndSceneLoad;
    public Action<float> OnSceneLoadProgress;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeSingleton();
        SetLightingState(LEDPlayState.ATTRACT);
        menuAudio = GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        UnLoadScenes();

        for (int i = 0; i < gameLoadButtons.Length; i++)
        {
            gameLoadButtons[i].interactable = true;
        }

        menuAudio.Play();


    }

    public void LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (sceneToLoad != null)
        {
            Debug.LogWarning($"SceneLoadManager.LoadScene already loading a scene. Please wait until loading is complete.");
            return;
        }

        currentSceneName = sceneName;
        
        isLoading = true;
        OnStartSceneLoad?.Invoke();
        
        sceneToLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        sceneToLoad.completed += OnLoadComplete;

        StartCoroutine(LoadingProgress());
    }

    private void OnLoadComplete(AsyncOperation operation)
    {
        sceneToLoad = null;
    }

    private void UnLoadScenes()
    {
        SceneManager.UnloadSceneAsync(currentSceneName);
        isLoading = true;
        OnStartSceneLoad?.Invoke();

        StartCoroutine(LoadingProgress(true, OnUnloadComplete));
    }

    private void OnUnloadComplete()
    {

    }

    private IEnumerator LoadingProgress(bool mainMenu = false, Action callback = null)
    {
        OnEndSceneLoad?.Invoke(mainMenu);
        
        SetLightingState(mainMenu?LEDPlayState.ATTRACT:LEDPlayState.PLAYING);
        
        isLoading = false;
        callback?.Invoke();
        yield return null;
    }

    public void SetLightingState(LEDPlayState state)
    {
        if (!arduinoLedControlEnabled) return;
        Debug.LogWarning($"SceneLoadManager.SetLightingState {state}");
        ArduinoLEDControl.SetState(state);
    }

    private bool InitializeSingleton()
    {
        if (singleton != null && singleton == this) return true;

        if (dontDestroyOnLoad)
        {
            if (singleton != null)
            {
                if(debugMessages) Debug.LogWarning($"Multiple {this.name} detected in the scene. Only one {this.name} can exist at a time. The duplicate {this.name} will be destroyed.");
                Destroy(this.gameObject);

                // Return false to not allow collision-destroyed second instance to continue.
                return false;
            }
            if(debugMessages) Debug.Log($"{this.name} created singleton (DontDestroyOnLoad)");
            singleton = this;
            if (Application.isPlaying) DontDestroyOnLoad(this);
        }
        else
        {
            if(debugMessages) Debug.Log($"{this.name} created singleton (ForScene)");
            singleton = this;
        }

        return true;
    }
}
