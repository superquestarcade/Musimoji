using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class AttractLoopManager : MonoBehaviour
{
    public bool debugMessages = false;
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
        if(debugMessages) Debug.Log("AttractLoopManager.StartGame");
        videoPlayer.Stop();
        OnStartGame?.Invoke();
        attractStateActive = false;
    }

    public void SetAttractState()
    {
        if(debugMessages) Debug.Log("AttractLoopManager.SetAttractState");
        ArduinoLEDControl.SetState(LEDPlayState.ATTRACT);
        videoPlayer.Play();
        attractStateActive = true;
        OnStartAttract?.Invoke();
    }
}
