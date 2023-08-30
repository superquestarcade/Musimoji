using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class AttractLoopManager : MonoBehaviourPlus
{
    [SerializeField] private VideoPlayer videoPlayer;
    public UnityEvent OnStartGame, OnStartAttract;
    private bool attractStateActive = true;
    private InputAction anyKeyAction;
    private float lastKeyPressTime;
    [SerializeField] private float attractStateTimeOut = 60f;

    private void Start()
    {
        anyKeyAction = new InputAction(binding: "/*/<button>");
        anyKeyAction.started += OnAnyKey;
        anyKeyAction.Enable();
        lastKeyPressTime = Time.time;
        SetAttractState();
    }

    private void Update()
    {
        if (attractStateActive) return;
        if(Time.time - lastKeyPressTime > attractStateTimeOut) SetAttractState();
    }

    private void OnAnyKey(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.started) return;
        lastKeyPressTime = Time.time;
        if (!attractStateActive) return;
        StartGame();
    }

    private void StartGame()
    {
        if(DebugMessages) Debug.Log("AttractLoopManager.StartGame");
        videoPlayer.Stop();
        OnStartGame?.Invoke();
        attractStateActive = false;
    }

    public void SetAttractState()
    {
        if(DebugMessages) Debug.Log("AttractLoopManager.SetAttractState");
        videoPlayer.Play();
        attractStateActive = true;
        OnStartAttract?.Invoke();
    }
}
