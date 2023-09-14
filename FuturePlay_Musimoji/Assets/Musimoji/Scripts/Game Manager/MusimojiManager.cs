using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class MusimojiManager : MonoBehaviourPlus
{
    #region Properties & Variables
    
    public bool debugStepIndicators = false;
    public MM_Sequencer sequencer;
    [SerializeField] private Color[] emojiColors;
    public Color[] EmojiColors => emojiColors;
    public bool PlayerControlActive { get; private set; } = false;

    [SerializeField] private int currentStep;
    public float sequenceDisplayRadius = 3f;

    private float StepAngle => (float)360/sequencer.SequenceStepCount;
    private float rotationTime, rotationDelta;
    public Transform centrePointTransform;
    public GameObject sequenceStepDisplayPrefab;
    public MusimojiSequenceStepDisplay[] sequenceStepDisplays;
    private MusimojiSequenceStepDisplay sequenceStepIndicator;
    private List<Vector3Int> repressList = new();

    [Header("Powerups")] 
    [SerializeField] private float powerupMinDuration, powerupMaxDuration;
    private float powerupDuration, powerupTimer;
    
    [Header("Audio sequence")]
    [SerializeField] private int[] currentSequenceData;
    
    [Header("Win emoji")]
    public SpriteRenderer winEmoji;
    public Sprite[] emojiSprites;
    private int winType = -1;

    public int EmojiCount => emojiSprites.Length;
    
    public float winDuration = 10f, winEmojiStartScale = 1f, winEmojiEndScale = 30f, resetAfterWin = 30f;
    private float winTimer = 0f;
    [SerializeField] private int[] winningSequence;
    private bool isWinning = false;
    
    [Header("Arduino LED Control")]
    [SerializeField] private bool arduinoLedControlEnabled = true;

    public bool LightingEnabled => arduinoLedControlEnabled;
    
    [Header("Events")]
    public UnityEvent<int> OnWinning, OnWin;
    public UnityEvent<float> OnWinIntensity;
    public UnityEvent OnStartGame;
    public UnityEvent OnEndGame;
    public UnityEvent<int> OnEmojiHit, OnRepressEmoji;
    
    #endregion

    #region Unity Functions

    private void Awake()
    {
        // if (!arduinoLedControlEnabled) ArduinoLEDControl.Singleton.enabled = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetupSequenceStepDisplays();
        // StartGame();
    }

    private void OnEnable()
    {
        sequencer.OnAnyStep.AddListener(OnSequenceStep);
    }

    private void OnDisable()
    {
        sequencer.OnAnyStep.RemoveListener(OnSequenceStep);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        rotationDelta = (sequencer.SequenceTimer / sequencer.SequenceDuration) * 360;
        RotateSequence(rotationDelta);
        // PowerUpUpdate();
        IsWinning();
    }

    #endregion
    
    #region Sequence Rotation

    private void SetupSequenceStepDisplays()
    {
        var sequenceDisplayObjectList = new List<MusimojiSequenceStepDisplay>();
        for (var i = 0; i < sequencer.SequenceStepCount; i++)
        {
            var stepDisplay = Instantiate(sequenceStepDisplayPrefab, CalculateOffsetPosition(i), Quaternion.identity,
                centrePointTransform).GetComponent<MusimojiSequenceStepDisplay>();
            sequenceDisplayObjectList.Add(stepDisplay);
            stepDisplay.SetAsIndicator(false);
            if(debugStepIndicators) stepDisplay.SetDebugStepNumber(i+1);
        }

        sequenceStepDisplays = sequenceDisplayObjectList.ToArray();
    }
    
    private Vector2 CalculateOffsetPosition(int stepCount)
    {
        if (centrePointTransform == null)
        {
            Debug.LogWarning($"No centre point transform assigned");
            return Vector2.zero;
        }
        
        var position = (Vector2)centrePointTransform.position;
        if (sequencer.SequenceStepCount < 1) return position;

        var angleSection = Mathf.PI * 2f / sequencer.SequenceStepCount;
        var angle = (Mathf.Deg2Rad * 90) + (stepCount * angleSection);
        Vector2 offset = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0) * sequenceDisplayRadius;
        return position + offset;
    }

    private void OnSequenceStep(int currentStep, int stepValue)
    {
        if(DebugLevel>DebugMessageLevel.MODERATE) Debug.Log($"MusimojiManager.OnSequenceStep {currentStep} value {stepValue}");
        this.currentStep = currentStep;
        CheckRepressRequests();
        // stepTime = 0;
    }

    private void RotateSequence(float angle)
    {
        centrePointTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    #endregion

    #region Player

    public void PlayerFireEmoji(int playerID, int emojiId)
    {
        var playerStepIndex = PlayerStepIndex(playerID);
        if (currentSequenceData[playerStepIndex] == emojiId) return;
        SetSequenceStep(playerStepIndex, emojiId);
        OnEmojiHit?.Invoke(emojiId);
        if(DebugMessages) Debug.Log($"MusimojiManager.PlayerFireEmoji player {playerID}, " +
                                    $"emoji {emojiId}, slot {playerStepIndex}");
        CheckWinCondition(playerStepIndex, SetWinningRun);
    }

    public bool PredictWinningShot(int playerIndex, int emojiId, float fireDuration)
    {
        var playerCurrentStep = PlayerStepIndex(playerIndex);
        var playerStepPredicted = PlayerStepPrediction(playerIndex, fireDuration);
        var isWinningShot = PredictWinCondition(playerStepPredicted, emojiId);
        if(DebugMessages) Debug.Log($"MusimojiManager.PredictWinningShot player {playerIndex} " +
                                    $"at step {playerCurrentStep} predicted step {playerStepPredicted} " +
                                    $"is winning shot {isWinningShot}");
        return isWinningShot;
    }
    
    //Player sequence index (0 - StepCount)
    private int PlayerStepIndex(int playerIndex)
    {
        var returnIndex = currentStep;
        double sequenceTimePoint;
            
        switch (playerIndex)
        {
            case 0:
                sequenceTimePoint = sequencer.SequenceDuration / 2;
                returnIndex += Mathf.FloorToInt((float)sequenceTimePoint / sequencer.StepDuration);
                break;
            case 1:
                sequenceTimePoint = sequencer.SequenceDuration * 5/6;
                returnIndex += Mathf.RoundToInt((float)sequenceTimePoint / sequencer.StepDuration);
                break;
            case 2:
                sequenceTimePoint = sequencer.SequenceDuration /6;
                returnIndex += Mathf.RoundToInt((float)sequenceTimePoint / sequencer.StepDuration);
                break;
        }
        
        if (returnIndex >= sequencer.SequenceStepCount) returnIndex -= sequencer.SequenceStepCount;
        return returnIndex;
    }

    private int PlayerStepPrediction(int playerIndex, float time)
    {
        var returnStep = PlayerStepIndex(playerIndex);
        var stepsIntoFuture = Mathf.RoundToInt(time / sequencer.StepDuration);
        returnStep += stepsIntoFuture;
        return returnStep;
    }

    #endregion
    
    #region Repress
    
    public void PlayerRepress(int playerId, int repressStepCount)
    {
        if (DebugLevel>DebugMessageLevel.MINIMAL)
            Debug.Log($"MM_Manager.PlayerRepress ({playerId}) currentStep {currentStep}");
        repressList.Add(new Vector3Int(currentStep, playerId, repressStepCount));
        var repressSequenceData = currentSequenceData[PlayerStepIndex(playerId)];
        OnRepressEmoji?.Invoke(repressSequenceData);
        PlayerFireEmoji(playerId, 0);
    }

    private void CheckRepressRequests()
    {
        if(repressList.Count<=0) return;
        if (DebugLevel>DebugMessageLevel.MINIMAL)
            Debug.Log($"MM_Manager.CheckRepressRequests ({repressList.Count})," +
                                        $" currentStep {currentStep}");
        var updatedRepressRequests = new List<Vector3Int>();
        foreach (var request in repressList)
        {
            // Remove request if current step has passed end step
            var requestEndStep = request.x + request.z;
            sequencer.WrapStepCount(ref requestEndStep);
            var stepsFrom = sequencer.StepsFromAtoB(currentStep, requestEndStep);
            if (DebugLevel>DebugMessageLevel.MINIMAL)
                Debug.Log($"MM_Manager.CheckRepressRequests currentStep {currentStep}, " +
                              $"requestEndStep {requestEndStep}, steps between {stepsFrom}");
            if (stepsFrom >= request.z)
            {
                // Don't repress & discard request
                if (DebugLevel>DebugMessageLevel.MINIMAL)
                    Debug.Log($"MM_Manager.CheckRepressRequests steps between {stepsFrom} >= " +
                                  $"stepCount {request.z}. Discarding request");
                continue;
            }
            
            // Repress player step
            if (DebugLevel>DebugMessageLevel.MINIMAL)
                Debug.Log(
                    $"MM_Manager.CheckRepressRequests repressing player {request.y}");
            PlayerFireEmoji(request.y, 0);
            
            // Add to updated requests
            updatedRepressRequests.Add(request);
        }
        repressList = updatedRepressRequests;
    }
    
    #endregion

    #region Sequence

    private void SetSequenceStep(int step, int emojiId)
    {
        sequenceStepDisplays[step].SetEmoji(emojiId);
        currentSequenceData[step] = emojiId;
        sequencer.SetStep(step,emojiId);
    }

    private void SetAudioSequenceData()
    {
        sequencer.SetSequence(currentSequenceData);
    }

    private int NextIndexInSequenceData(int[] sequence, int index)
    {
        if (index + 1 >= sequence.Length)
        {
            if(DebugMessages) Debug.Log($"NextIndexInSequenceData for index {index} >= sequence.Length. Wrapping to min index.");
            return index + 1 - sequence.Length;
        }
        if(DebugMessages) Debug.Log($"NextIndexInSequenceData for index {index} < sequence.Length. Returning {index + 1}");
        return index + 1;
    }

    private int PreviousIndexInSequenceData(int[] sequence, int index)
    {
        if (index - 1 < 0)
        {
            if(DebugMessages) Debug.Log($"PreviousIndexInSequenceData for index {index} < 0. Wrapping to max index {sequence.Length - 1}");
            return sequence.Length - 1;
        }
        if(DebugMessages) Debug.Log($"PreviousIndexInSequenceData for index {index} >= 0. Returning {index - 1}");
        return index - 1;
    }

    public void StartGame()
    {
        ResetGame();
        OnStartGame?.Invoke();
    }

    private void ResetGame()
    {
        EndGame();
        winEmoji.size = Vector2.one;
        winEmoji.enabled = false;
        sequencer.Restart();
        currentSequenceData = new int[sequencer.SequenceStepCount];
        SetAudioSequenceData();
        foreach(var s in sequenceStepDisplays) s.SetEmoji(0);
        PlayerControlActive = true;
    }

    #endregion
    
    #region Power Up

    private void PowerUpUpdate()
    {
        // Update timer
        powerupTimer += Time.deltaTime;
        if (powerupTimer < powerupDuration) return;
        // Add power up
        AddPowerUp();
        ResetPowerupTimer();
    }

    private void AddPowerUp()
    {
        Debug.LogWarning("MM_Manager.AddPowerup");
        // Find available empty slots
        
        // If no slots do nothing
        
        // Pick a random slot & add power up
        
    }

    private void ResetPowerupTimer()
    {
        powerupTimer = 0;
        powerupDuration = Random.Range(powerupMinDuration, powerupMaxDuration);
    }
    
    #endregion
    
    #region Pods

    public int GetPodStepValue(int podStepIndex)
    {
        var returnIndex = currentStep+1;
        double sequenceTimePoint = (sequencer.SequenceDuration / sequencer.SequenceStepCount) * podStepIndex;
        returnIndex += Mathf.FloorToInt((float)sequenceTimePoint / sequencer.StepDuration);
        if (returnIndex >= sequencer.SequenceStepCount) returnIndex -= sequencer.SequenceStepCount;
        return currentSequenceData[returnIndex];
    }
    
    #endregion

    #region Winning

    private bool CheckWinCondition(int sequenceIndex, Action<int,int,int,int> winCallback = null)
    {
        //Incoming step will be 1 or higher
        if(DebugMessages) Debug.Log($"CheckWinCondition step index {sequenceIndex}");
        if (sequenceIndex < 0 || sequenceIndex >= sequencer.SequenceStepCount)
        {
            Debug.LogError($"MusiMojiManager.CheckWinCondition sequenceIndex {sequenceIndex} out of range {sequencer.SequenceStepCount}");
            return false;
        }
        // Find 2 steps before fired step so we can check before & after
        var secondIndex = PreviousIndexInSequenceData(currentSequenceData, sequenceIndex);
        var firstIndex = PreviousIndexInSequenceData(currentSequenceData, secondIndex);
        if(CheckWinTriplet(currentSequenceData, firstIndex, secondIndex, sequenceIndex, winCallback)) return true;
        for (var s = 0; s < 2; s++)
        {
            var thisIndex = NextIndexInSequenceData(currentSequenceData, firstIndex + s);
            var nextIndex = NextIndexInSequenceData(currentSequenceData, thisIndex);
            var thirdIndex = NextIndexInSequenceData(currentSequenceData, nextIndex);
            
            if(CheckWinTriplet(currentSequenceData, thisIndex, nextIndex, thirdIndex, winCallback)) return true;
        }
        if(DebugMessages) Debug.Log($"MusiMojiManager.CheckWinCondition sequenceIndex {sequenceIndex} is not a winner");
        CheckWinningRun(sequenceIndex);
        return false;
    }

    private bool PredictWinCondition(int sequenceIndex, int emojiId)
    {
        if(DebugMessages) Debug.Log($"PredictWinCondition step index {sequenceIndex} emoji {emojiId}");
        if(sequenceIndex < 0 || sequenceIndex >= currentSequenceData.Length) return false;
        var predictedSequenceData = new int[currentSequenceData.Length];
        Array.Copy(currentSequenceData, predictedSequenceData, currentSequenceData.Length);
        predictedSequenceData[sequenceIndex] = emojiId;
        // Find 2 steps before fired step so we can check before & after
        var secondIndex = PreviousIndexInSequenceData(predictedSequenceData, sequenceIndex);
        var firstIndex = PreviousIndexInSequenceData(predictedSequenceData, secondIndex);
        if(CheckWinTriplet(predictedSequenceData, firstIndex, secondIndex, sequenceIndex)) return true;
        for (var s = 0; s < 2; s++)
        {
            var thisIndex = NextIndexInSequenceData(predictedSequenceData, firstIndex + s);
            var nextIndex = NextIndexInSequenceData(predictedSequenceData, thisIndex);
            var thirdIndex = NextIndexInSequenceData(predictedSequenceData, nextIndex);
            
            if(CheckWinTriplet(predictedSequenceData, thisIndex, nextIndex, thirdIndex)) return true;
        }
        return false;
    }

    private bool CheckWinTriplet(int[] sequenceData, int a, int b, int c, Action<int,int,int,int> winCallback = null)
    {
        if(DebugMessages) Debug.Log($"CheckWinTriplet indexes {a}, {b}, {c}");
        if (sequenceData[a] == 0) return false;
        if (sequenceData[a] != sequenceData[b] || sequenceData[b] != sequenceData[c]) return false;
        winCallback?.Invoke(sequenceData[a], a, b, c);
        return true;

    }

    private void CheckWinningRun(int index)
    {
        foreach (var step in winningSequence)
        {
            if (step != index) continue;
            
            if(DebugMessages) Debug.Log($"CheckWinningRun clear step {index}");
            StopWinning();
            return;
        }
        if(DebugMessages) Debug.Log($"CheckWinningRun step {index}");
    }

    private void CheckWinningRunType()
    {
        if (winningSequence == null) return;
        foreach (var w in winningSequence)
        {
            if (winType > 0 && currentSequenceData[w] != winType) continue;

            if(DebugMessages) Debug.Log($"ClearWinningRun clear step {w}");
            currentSequenceData[w] = 0;
            sequenceStepDisplays[w].SetEmoji(0);
        }
        StopWinning();
    }

    private void StopWinning()
    {
        if(DebugMessages) Debug.Log($"StopWinning array clear {string.Join(",", winningSequence)}");
        winningSequence = Array.Empty<int>();
        winEmoji.enabled = false;
        winEmoji.transform.localScale = Vector2.one;
        isWinning = false;
        winTimer = 0f;
        OnWinning.Invoke(0);
    }

    private void SetWinningRun(int winEmojiType, int a, int b, int c)
    {
        if(winEmojiType==0) return;
        CheckWinningRunType();
        if (winningSequence is {Length: 3} && 
            winningSequence[0] == a && winningSequence[1] == b && winningSequence[2] == c) return;
        winType = winEmojiType;
        OnWinning.Invoke(winType);
        isWinning = true;
        winTimer = 0f;
        winningSequence = new[] {a, b, c};
        if(DebugMessages) Debug.Log($"CheckWinCondition WINNING! {string.Join(", ", winningSequence)} type {winType}");
        winEmoji.sprite = emojiSprites[winType-1];
        winEmoji.enabled = true;
    }

    private void IsWinning()
    {
        if (!isWinning) return;
        OnWinIntensity?.Invoke(winTimer/winDuration);
        if (winTimer >= winDuration)
        {
            WinComplete();
            OnWinIntensity?.Invoke(0);
            return;
        }
        var winFactor = winTimer / winDuration;
        var winScale = winEmojiStartScale + ((winEmojiEndScale - winEmojiStartScale) * winFactor);
        winEmoji.transform.localScale = Vector2.one * winScale;
        winTimer += Time.deltaTime;
    }

    private void WinComplete()
    {
        if(DebugMessages) Debug.Log($"WinComplete");
        PlayerControlActive = false;
        OnWin?.Invoke(winType);
        isWinning = false;
        winTimer = 0f;
        StartCoroutine(ResetAfterWin());
    }

    private IEnumerator ResetAfterWin()
    {
        yield return new WaitForSeconds(resetAfterWin);
        ResetGame();
        EndGame();
    }

    private void EndGame()
    {
        OnEndGame?.Invoke();
    }

    #endregion
}
