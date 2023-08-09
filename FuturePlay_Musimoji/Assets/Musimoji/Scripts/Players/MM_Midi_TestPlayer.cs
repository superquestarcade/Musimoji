using UnityEngine;
using UnityEngine.UI;

namespace Musimoji
{
    public class MM_Midi_TestPlayer : MonoBehaviourPlus
    {
        [SerializeField] private Image emojiImageContainer;
        private ImageSizeTween imageSizeTween;
        [SerializeField] private EmojiNoteMap[] emojiRefs;
        private int currentEmoji;

        private void Start()
        {
            imageSizeTween = emojiImageContainer.GetComponent<ImageSizeTween>();
        }
        
        public void SetEmoji(Note value, float velocity = 0.5f)
        {
            if (!GetEmojiFromNote(value, out var s))
            {
                Debug.LogError($"MM_MIDI_TestController.SetEmoji no sprite found ({value})");
                return;
            }
            try
            {
                emojiImageContainer.sprite = s;
            }
            catch
            {
                Debug.LogError($"MM_MIDI_TestController.SetEmoji no sprite found ({value})");
            }
            imageSizeTween.TweenStart(velocity, 1f);
        }

        private bool GetEmojiFromNote(Note note, out Sprite sprite)
        {
            foreach (var emojiMap in emojiRefs)
            {
                if (emojiMap.noteGroup.Contains(note))
                {
                    sprite = emojiMap.emojiSprite;
                    return true;
                }
            }

            sprite = null;
            return false;
        }
    }
}