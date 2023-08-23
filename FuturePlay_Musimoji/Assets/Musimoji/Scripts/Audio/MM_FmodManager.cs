using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MM_FmodManager : MonoBehaviourPlus
{
    [SerializeField] private MusimojiManager mmManager;
    public EventReference startGameEventReference, restartGameEventReference;
    public EventReference musicControllerEventReference;
    private EventInstance musicControllerEventInstance;
    public StudioGlobalParameterTrigger[] intensityParametersTriggers;
    [SerializeField] private float[] intensities;
    public EventReference[] winEventReferences;
    public EventReference[] playerExpressEventReferences;

    private void Start()
    {
        intensities = new float[mmManager.EmojiCount];
        PlayOneShot(startGameEventReference, gameObject);
        SetAudioEventActive(musicControllerEventReference, ref musicControllerEventInstance, gameObject, true);
    }

    private void OnEnable()
    {
        mmManager.sequencer.OnSetSequenceData.AddListener(OnSetSequenceData);
        mmManager.OnStartGame.AddListener(RestartGame);
        mmManager.OnWin.AddListener(OnWin);
    }

    private void OnDisable()
    {
        mmManager.sequencer.OnSetSequenceData.RemoveListener(OnSetSequenceData);
        mmManager.OnStartGame.RemoveListener(RestartGame);
        mmManager.OnWin.RemoveListener(OnWin);
    }

    private void OnSetSequenceData(int[] sequenceData)
    {
        if(DebugMessages) Debug.Log($"MM_FmodManager.OnSetSequenceData {sequenceData.Length}");

        for (var i = 0; i < intensities.Length; i++)
        {
            var intensity = mmManager.sequencer.GetEmojiIntensity(i + 1);
            intensities[i] = intensity;
            intensityParametersTriggers[i].Value = intensity;
            intensityParametersTriggers[i].TriggerParameters();
        }
    }

    private void RestartGame()
    {
        if(DebugMessages) Debug.Log("MM_FmodManager.RestartGame");
        PlayOneShot(restartGameEventReference, gameObject);
    }

    private void OnWin(int winningEmojiIndex)
    {
        if(DebugMessages) Debug.Log($"MM_FmodManager.OnWin {winningEmojiIndex}");
        SetAudioEventActive(musicControllerEventReference, ref musicControllerEventInstance, gameObject, false, STOP_MODE.IMMEDIATE);
        PlayOneShot(winEventReferences[winningEmojiIndex-1], gameObject);
    }

    public void OnPlayerExpressEmoji(int player, int emojiId)
    {
        if (emojiId == 0) return;
        if(DebugMessages) Debug.Log($"MM_FmodManager.OnPlayerExpressEmoji player {player}, emoji {emojiId}");
        PlayOneShot(playerExpressEventReferences[emojiId-1], gameObject);
    }

    #region Play Functions

    private bool IsPlaying(EventInstance instance) 
        {
            PLAYBACK_STATE state;   
            instance.getPlaybackState(out state);
            return state != PLAYBACK_STATE.STOPPED;
        }

    private void PlayOneShot(EventReference eventPath, GameObject attachToObject)
        {
            if(eventPath.IsNull) return;

            if (attachToObject == null)
            {
                Debug.LogWarning($"AudioManager.PlayOneShot null object reference for event {eventPath}");
                return;
            }
        
            if(DebugMessages) Debug.Log("Playing audio event: " + eventPath);
            RuntimeManager.PlayOneShotAttached(eventPath.Guid, attachToObject);
        }

    private void SetAudioEventActive(EventReference eventPath, ref EventInstance eventInstance, GameObject audioSource, bool active, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
        {
            if(eventPath.IsNull) return;
        
            if (audioSource == null)
            {
                audioSource = gameObject;
            }
        
            if (active)
            {
                if(IsPlaying(eventInstance)) return;
                if(DebugMessages) Debug.Log("AudioManager.SetAudioEventActive Starting audio instance at path " + eventPath);
            
                //Only start new event instance if not already playing
                if (!eventInstance.isValid()) eventInstance = RuntimeManager.CreateInstance(eventPath);
            
                Rigidbody rb = null;
                RuntimeManager.AttachInstanceToGameObject(eventInstance, audioSource.transform, rb);
                eventInstance.start();
            }
            else
            {
                if(!IsPlaying(eventInstance)) return;
                if(DebugMessages) Debug.Log("AudioManager.SetAudioEventActive Stopping audio instance for path " + eventPath);
                eventInstance.stop(stopMode);
                // eventInstance.release();
            }
        }

        #endregion
}
