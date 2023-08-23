using UnityEngine;

namespace Musimoji
{
    public class MM_EmojiDisplay : MonoBehaviourPlus
    {
        [SerializeField] private GameObject[] emojis;
        public int EmojiDisplayCount => emojis.Length;

        public void SetEmoji(int emojiIndex)
        {
            if (emojiIndex == 0)
            {
                foreach(var eObj in emojis) eObj.SetActive(false);
                return;
            }
            for (var e = 0; e < emojis.Length; e++)
            {
                var eObject = emojis[e];
                eObject.SetActive(e==emojiIndex-1);
            }
        }
    }
}