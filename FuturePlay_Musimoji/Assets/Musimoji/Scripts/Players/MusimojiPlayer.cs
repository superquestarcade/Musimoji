using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Musimoji;
using Musimoji.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MusimojiPlayer : MonoBehaviourPlus
{
    [Header("Settings")]
    public MusimojiManager manager;
    [SerializeField] private MM_FmodManager mmFmodManager;
    [SerializeField] private GameObject emojiFireObject;
    [SerializeField] private List<GameObject> firedObjects = new List<GameObject>();
    private bool canFire = true, canRepress = true;
    [SerializeField] private float repressBeamDestroyDelay = 0.2f, repressBeamActiveDuration = 2f, repressBeamFireAgainDelay = 5f;
    [SerializeField] private float destroyFiredEmojiDelay = 1f, fireAgainDelay = 3.5f;
    [SerializeField] private int repressStepCount = 3;
    [SerializeField] private EmojiNoteMap fireNoteMap = new(), repressNoteMap = new();
    [SerializeField] private EmojiRefs_SO emojiRefs;
    
    [Header("Emojis")]
    public int playerID;
    public int selectedEmoji { get; private set; } = 1;

    [FormerlySerializedAs("playerEmojiDisplay")] [SerializeField] private MM_EmojiDisplay emojiDisplay;
    // public SpriteRenderer emojiDisplay;
    public Sprite emptyEmoji;
    public Sprite[] emojiSprites;

    public MM_PlayerBackground playerBackground;
    [SerializeField] private Color playerBgColourMultiplier = Color.white;
    [SerializeField] private int scrambleEmojiCycles = 6;
    [SerializeField] private float scrambleCycleDuration = 2f;
    private int emojiScrambleCount = 0;
    private List<int> emojiScrambleList;
    
    [Header("Repress beam")]
    public GameObject repressBeam;
    
    [Header("Human/ Bot control")]
    [SerializeField] private bool isHuman = true;
    public bool IsHuman => isHuman;
    
    public Action<bool> OnSetHuman;

    public float botActiveTimeout = 30;
    [SerializeField] private float botActiveTimer = 0;

    public UnityEvent EmojiExpressEvent, EmojiRepressEvent, EmojiHitEvent, EmojiReloadEvent;
    public UnityEvent<int> EmojiChangeEvent;

    private void Start()
    {
        UpdateEmojiDisplay();
    }

    private void Update()
    {
        CheckIsHuman();
    }

    private void OnEnable()
    {
        manager.OnRepressEmoji.AddListener(OnRepressEmoji);
    }

    private void OnDisable()
    {
        manager.OnRepressEmoji.RemoveListener(OnRepressEmoji);
    }

    #region Player Management

    public void InitializeHuman()
    {
        if (isHuman) return;
        
        UpdateEmojiDisplay();
        StartCoroutine(StartActiveDelay());
        isHuman = true;
        OnSetHuman?.Invoke(isHuman);
        ResetBotTimer();
    }

    private void InitializeBot()
    {
        isHuman = false;
        UpdateEmojiDisplay();
        OnSetHuman?.Invoke(isHuman);
        ResetBotTimer();
    }

    private void CheckIsHuman()
    {
        if(isHuman) HumanInactiveTimer();
    }

    private void HumanInactiveTimer()
    {
        if(botActiveTimer>=botActiveTimeout) InitializeBot();

        botActiveTimer += Time.deltaTime;
    }

    public void ResetBotTimer() => botActiveTimer = 0f;

    #endregion
    
    #region Note Based Input

    public void OnNoteDown(Note note, float velocity)
    {
        if (fireNoteMap.Contains(note))
        {
            if(DebugMessages) Debug.Log($"MM_Player.OnNoteDown firing ({note})");
            FireEmoji();
            return;
        }

        if (repressNoteMap.Contains(note))
        {
            if(DebugMessages) Debug.Log($"MM_Player.OnNoteDown repressing ({note})");
            FireRepress();
            return;
        }
        if(DebugMessages) Debug.Log($"MM_Player.OnNoteDown updating emoji display ({note})");
        UpdateEmojiDisplay(emojiRefs.GetIndex(note));
    }
    
    #endregion

    #region Firing

    public void FireEmoji()
    {
        if (!manager.PlayerControlActive || !canFire) return;
        if (selectedEmoji == 0) SetEmoji(Random.Range(1,emojiDisplay.EmojiDisplayCount));
        EmojiExpressEvent.Invoke();
        mmFmodManager.OnPlayerExpressEmoji(playerID, selectedEmoji);
        canFire = false;
        var newObject = Instantiate(emojiFireObject, transform.position, transform.rotation, transform);
        var setSprite = emptyEmoji;
        if (selectedEmoji > 0)
        {
            setSprite = emojiSprites[selectedEmoji - 1];
        }
        
        newObject.GetComponent<MusimojiBullet>().SetSprite(setSprite);
        firedObjects.Add(newObject);
        StartCoroutine(DelayDestroyFiredObject(newObject, SetSequenceEmoji));
    }

    private IEnumerator DelayDestroyFiredObject(GameObject firedObject, Action callback = null)
    {
        emojiDisplay.SetEmoji(0);
        yield return new WaitForSeconds(destroyFiredEmojiDelay);
        firedObjects.Remove(firedObject);
        Destroy(firedObject);
        EmojiHitEvent?.Invoke();
        callback?.Invoke();
        yield return new WaitForSeconds(fireAgainDelay);
        SetEmoji(selectedEmoji);
        canFire = true;
        EmojiReloadEvent?.Invoke();
    }

    private IEnumerator StartActiveDelay()
    {
        yield return new WaitForSeconds(1);
        canFire = true;
    }

    private void SetSequenceEmoji()
    {
        if(DebugMessages) Debug.Log($"Player set emoji {selectedEmoji}");
        manager.PlayerFireEmoji(playerID, selectedEmoji);
    }

    public bool PredictWinningShot()
    {
        var willWin = manager.PredictWinningShot(playerID, selectedEmoji, destroyFiredEmojiDelay);
        if(DebugMessages) Debug.Log($"MusimojiPlayer.PredictWinningShot willWin {willWin}");
        return willWin;
    }

    #endregion
    
    #region Selecting Emoji
    
    public void NextEmoji()
    {
        if (!manager.PlayerControlActive || !canFire) return;
        selectedEmoji += 1;
        if (selectedEmoji <= 0) selectedEmoji = emojiSprites.Length;
        if (selectedEmoji > emojiSprites.Length) selectedEmoji = 1;
        if(DebugMessages) Debug.Log($"NextEmoji Player {playerID}: Selecting emoji {selectedEmoji}");
        UpdateEmojiDisplay();
    }

    /// <summary>
    /// Set the current emoji & update for this player
    /// </summary>
    /// <param name="value">Emoji type (not index) starts at 1</param>
    private void SetEmoji(int value)
    {
        if (value < 0 || value > emojiSprites.Length)
        {
            Debug.LogError($"MM_Player.SetEmoji no sprite available for this emoji index. Abort! ({value})");
            return;
        }
        selectedEmoji = value;
        if (selectedEmoji == 0)
        {
            emojiDisplay.SetEmoji(0);
            if (playerBackground == null) return;
            playerBackground.SetBgColor(Color.grey);
            return;
        }
        UpdateEmojiDisplay();
    }

    private void UpdateEmojiDisplay()
    {
        if(DebugMessages) Debug.Log($"SetEmoji Player {playerID}: Selecting emoji {selectedEmoji}");
        emojiDisplay.SetEmoji(selectedEmoji);

        if (playerBackground != null)
        {
            var bgColour = playerBgColourMultiplier * manager.EmojiColors[selectedEmoji - 1];
            playerBackground.SetBgColor(bgColour);
        }
        
        EmojiChangeEvent?.Invoke(selectedEmoji);
    }

    private void UpdateEmojiDisplay(int emojiType)
    {
        if (emojiType < 0) return;
        if(DebugMessages) Debug.Log($"SetEmoji Player {playerID}: Selecting emoji {emojiType}");
        selectedEmoji = emojiType;
        emojiDisplay.SetEmoji(selectedEmoji);

        if (playerBackground == null) return;
        var bgColour = playerBgColourMultiplier * manager.EmojiColors[selectedEmoji - 1];
        playerBackground.SetBgColor(bgColour);
    }

    private void CycleToRandomEmoji()
    {
        emojiScrambleCount = 0;
        emojiScrambleList = new List<int>();
        for (var i = 1; i <= emojiSprites.Length; i++) emojiScrambleList.Add(i);
        var delay = scrambleCycleDuration / scrambleEmojiCycles;
        StartCoroutine(SelectRandomEmojiDelay(emojiScrambleList.ToArray(), delay, NextScrambleEmoji));
    }

    private void NextScrambleEmoji(int previousEmojiValue)
    {
        if (emojiScrambleCount >= scrambleEmojiCycles) return;
        emojiScrambleList.Remove(previousEmojiValue);
        var delay = scrambleCycleDuration / scrambleEmojiCycles;
        StartCoroutine(SelectRandomEmojiDelay(emojiScrambleList.ToArray(), delay, NextScrambleEmoji));
    }

    private IEnumerator SelectRandomEmojiDelay(int[] emojiValues, float delay, Action<int> callback)
    {
        emojiScrambleCount++;
        var emojiValue = emojiValues[Random.Range(0, emojiValues.Length)];
        SetEmoji(emojiValue);
        yield return new WaitForSeconds(delay);
        callback?.Invoke(emojiValue);
        yield return null;
    }
    
    #endregion
    
    #region Repress

    public void FireRepress()
    {
        if (!manager.PlayerControlActive || !canRepress) return;
        StartCoroutine(DelayRepress(Repress));
    }

    private IEnumerator DelayRepress(Action callback = null)
    {
        canRepress = false;
        repressBeam.SetActive(true);
        EmojiRepressEvent?.Invoke();
        yield return new WaitForSeconds(repressBeamDestroyDelay);
        callback?.Invoke();
        yield return new WaitForSeconds(repressBeamActiveDuration);
        repressBeam.SetActive(false);
        yield return new WaitForSeconds(repressBeamFireAgainDelay);
        canRepress = true;
    }

    private void Repress()
    {
        if(DebugMessages) Debug.Log($"Repress {playerID}");
        manager.PlayerRepress(playerID, repressStepCount);
    }

    private void OnRepressEmoji(int emojiId)
    {
        if (selectedEmoji != emojiId) return;
        if(DebugMessages) Debug.Log($"MM_Player.OnRepressEmoji selectedEmoji {selectedEmoji}");
        CycleToRandomEmoji();
    }
    
    #endregion
    
}
