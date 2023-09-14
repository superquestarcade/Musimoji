using System;
using System.Collections;
using UnityEngine;

namespace Musimoji.Scripts.Lighting
{
    public class LightingControl : MonoBehaviourPlus
    {
        private LEDPlayState previousState;
        private Coroutine repressCoroutine, hitCoroutine;
        
        [SerializeField] private MusimojiManager manager;
        [SerializeField] private AttractLoopManager attractLoopManager;

        [Header("Settings")] 
        [SerializeField] private float hitDuration = 2f;
        [SerializeField] private float repressDuration = 2f;

        private void Start()
        {
            if (!manager.LightingEnabled) return;
            attractLoopManager.OnStartAttract.AddListener(OnAttract);
            manager.OnStartGame.AddListener(OnStartGame);
            manager.OnEmojiHit.AddListener(OnEmojiHit);
            manager.OnRepressEmoji.AddListener(OnRepressEmoji);
            manager.OnWinning.AddListener(OnWinning);
            manager.OnWin.AddListener(OnWin);
            manager.OnEndGame.AddListener(OnEndGame);
        }

        #region Triggers
        
        private void OnAttract()
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log("LightingControl.OnAttract");
            ArduinoLEDControl.SetState(LEDPlayState.ATTRACT);
        }

        private void OnStartGame()
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log("LightingControl.OnStartGame");
            previousState = LEDPlayState.PLAYING;
            ArduinoLEDControl.SetState(LEDPlayState.PLAYING);
        }
        
        private void OnEmojiHit(int emojiIndex)
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log($"LightingControl.OnEmojiHit {emojiIndex}");
            if (emojiIndex is <= 0 or > 8) return;
            if(hitCoroutine!=null) StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(OnHitDelay(emojiIndex));
        }

        private IEnumerator OnHitDelay(int emojiIndex)
        {
            ArduinoLEDControl.SetState(ArduinoLEDControl.GetStaticStateFromEmoji(emojiIndex));
            yield return new WaitForSeconds(hitDuration);
            ArduinoLEDControl.SetState(previousState);
        }
        
        private void OnRepressEmoji(int emojiIndex)
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log("LightingControl.OnRepressEmoji");
            if (emojiIndex is <= 0 or > 8) return;
            if(repressCoroutine!=null) StopCoroutine(repressCoroutine);
            repressCoroutine = StartCoroutine(OnRepressDelay());
        }

        private IEnumerator OnRepressDelay()
        {
            ArduinoLEDControl.SetState(LEDPlayState.CHAOS);
            yield return new WaitForSeconds(repressDuration);
            ArduinoLEDControl.SetState(previousState);
        }
        
        private void OnWinning(int emojiIndex)
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log($"LightingControl.OnWinning {emojiIndex}");
            if (emojiIndex == 0)
            {
                ArduinoLEDControl.SetState(previousState);
                return;
            }
            previousState = ArduinoLEDControl.GetBlinkingStateFromEmoji(emojiIndex);
            ArduinoLEDControl.SetState(previousState);
        }

        private void OnWin(int emojiIndex)
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log($"LightingControl.OnWin {emojiIndex}");
            ArduinoLEDControl.SetState(ArduinoLEDControl.GetBreathingStateFromEmoji(emojiIndex));
        }
        
        private void OnEndGame()
        {
            if(DebugLevel>=DebugMessageLevel.MINIMAL) Debug.Log($"LightingControl.OnEndGame");
            previousState = LEDPlayState.STANDBY;
            ArduinoLEDControl.SetState(previousState);
        }
        #endregion
    }
}