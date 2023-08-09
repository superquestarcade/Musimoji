using System.Linq;
using UnityEngine;

namespace Musimoji
{
    [CreateAssetMenu(menuName = "Create NoteGroup_SO", fileName = "NoteGroup_SO", order = 0)]
    public class NoteGroup_SO : ScriptableObject
    {
        public Note[] notes;

        public bool Contains(Note note)
        {
            return notes.Contains(note);
        }
    }
}

