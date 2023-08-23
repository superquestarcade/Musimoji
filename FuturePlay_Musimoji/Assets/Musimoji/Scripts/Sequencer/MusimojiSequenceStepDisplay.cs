using System.Collections;
using TMPro;
using UnityEngine;

public class MusimojiSequenceStepDisplay : MonoBehaviourPlus
{
    public int EmojiID { get; private set; } = 0;

    public Sprite[] emojiSprites;
    public Color[] emojiHitColors;

    [SerializeField] private Sprite powerupSprite;
    [SerializeField] private Color powerupColor = Color.black;

    public SpriteRenderer background, foreground, hitSlotDisplayFg, hitSlotDisplayBg;

    public Color backgroundDefault, backgroundSelected;

    public float hitDuration = 0.5f;

    [SerializeField] private Canvas stepNumberCanvas;
    [SerializeField] private TMP_Text debugStepNumberText;

    [SerializeField] private bool vfxActive = true;
    [SerializeField] private MM_EmojiVfx hitVfx;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    public void StepActive(bool value)
    {
        background.color = value ? backgroundSelected:backgroundDefault;
    }

    public void SetAsIndicator(bool value)
    {
        background.color = value ? backgroundSelected:backgroundDefault;
        foreground.enabled = !value;
    }

    public void SetDebugStepNumber(int step)
    {
        stepNumberCanvas.gameObject.SetActive(true);
        debugStepNumberText.text = step.ToString();
    }

    /// <summary>
    /// Activates a particular emoji in this slot.
    /// </summary>
    /// <param name="value">0 = empty</param>
    public void SetEmoji(int value)
    {
        if(DebugMessages) Debug.Log($"MusimojiSequenceStepDisplay.SetEmoji ({value})");
        if (value == 99)
        {
            SetPowerup();
            return;
        }
        if (value < 0 || value > emojiSprites.Length)
        {
            Debug.LogError("MusimojiSequenceStepDisplay.SetEmoji emoji value has no corresponding sprite");
            return;
        }
        EmojiID = value;
        if (value == 0)
        {
            foreground.sprite = null;
            background.enabled = true;
            return;
        }
        foreground.sprite = emojiSprites[value-1];
        background.enabled = false;
        StartCoroutine(SetHitActive(value - 1, hitDuration));
    }

    private void SetPowerup()
    {
        if(DebugMessages) Debug.Log("MusimojiSequenceStepDisplay.SetPowerup");
        EmojiID = 99;
        foreground.sprite = powerupSprite;
        background.enabled = false;
    }

    private IEnumerator SetHitActive(int colorIndex, float duration)
    {
        hitSlotDisplayBg.color = emojiHitColors[colorIndex];
        hitSlotDisplayBg.enabled = true;
        hitSlotDisplayFg.enabled = true;
        
        if(vfxActive && hitVfx!=null) hitVfx.TriggerGroup(colorIndex);
        
        yield return new WaitForSeconds(duration);
        hitSlotDisplayBg.enabled = false;
        hitSlotDisplayFg.enabled = false;
    }
}
