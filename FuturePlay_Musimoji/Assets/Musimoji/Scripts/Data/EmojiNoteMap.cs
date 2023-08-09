using System;
using UnityEngine;

namespace Musimoji
{
    [Serializable]
    public class EmojiNoteMap
    {
        public int index = -1;
        public NoteGroup_SO noteGroup;
        public Sprite emojiSprite;

        public bool Contains(Note note)
        {
            return noteGroup.Contains(note);
        }
    }
}

