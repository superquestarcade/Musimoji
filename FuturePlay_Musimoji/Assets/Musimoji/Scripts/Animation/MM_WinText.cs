using System;
using TMPro;
using UnityEngine;

namespace Musimoji
{
    public class MM_WinText: MonoBehaviour
    {
        public MusimojiManager manager;
        
        public GameObject winCanvas;
        public TMP_Text winText;

        public string joyText = "",
            trustText = "",
            fearText = "",
            surpriseText = "",
            sadnessText = "",
            disgustText = "",
            angerText = "",
            anticipationText = "";

        private void Start()
        {
            OnReset();
        }

        public void OnWin(int emojiIndex)
        {
            winText.color = manager.EmojiColors[emojiIndex-1];
            
            switch (emojiIndex)
            {
                case 1: //joy
                    winText.text = joyText;
                    break;
                case 2: //trust
                    winText.text = trustText;
                    break;
                case 3: //fear
                    winText.text = fearText;
                    break;
                case 4: //surprise
                    winText.text = surpriseText;
                    break;
                case 5: //sadness
                    winText.text = sadnessText;
                    break;
                case 6: //disgust
                    winText.text = disgustText;
                    break;
                case 7: //anger
                    winText.text = angerText;
                    break;
                case 8: //anticipation
                    winText.text = anticipationText;
                    break;
                default:
                    Debug.LogError($"MM_WinText.OnWin index out of range {emojiIndex}");
                    break;
            }
            
            winText.outlineColor = Color.black;
            winText.outlineWidth = 0.3f;
            
            winCanvas.SetActive(true);
        }

        public void OnReset()
        {
            winCanvas.SetActive(false);
        }
    }
}