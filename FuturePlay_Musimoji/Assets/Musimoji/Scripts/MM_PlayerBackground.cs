using UnityEngine;

namespace Musimoji.Scripts
{
    public class MM_PlayerBackground : MonoBehaviour
    {
        public SpriteRenderer[] playerBgSprites;

        public void SetBgColor(Color color)
        {
            foreach (var s in playerBgSprites) s.color = color;
        }
    }
}