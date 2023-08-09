using System.Linq;
using UnityEngine;

namespace Musimoji
{
    [CreateAssetMenu(menuName = "Create EmojiRefs_SO", fileName = "EmojiRefs_SO", order = 0)]
    public class EmojiRefs_SO : ScriptableObject
    {
        public EmojiNoteMap[] noteMaps;
        
        public int GetIndex(Note note)
        {
            foreach (var emojiMap in noteMaps)
            {
                if (emojiMap.noteGroup.Contains(note))
                {
                    return emojiMap.index;
                }
            }
            return -1;
        }

        public Sprite GetSprite(Note note)
        {
            foreach (var emojiMap in noteMaps)
            {
                if (emojiMap.noteGroup.Contains(note))
                {
                    return emojiMap.emojiSprite;
                }
            }
            return null;
        }
        
        public Sprite GetSprite(int index)
        {
            foreach (var emojiMap in noteMaps)
            {
                if (emojiMap.index == index) return emojiMap.emojiSprite;
            }
            return null;
        }
    }
}