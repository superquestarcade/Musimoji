using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Musimoji.Scripts
{
    public class MinisNoteInputMapper : MonoBehaviourPlus
    {
        [SerializeField] private PlayerInput playerInput;

        private void Awake()
        {
            playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
        }

        private void OnControlsChanged(PlayerInput playerInput)
        {
            
        }
    }
}