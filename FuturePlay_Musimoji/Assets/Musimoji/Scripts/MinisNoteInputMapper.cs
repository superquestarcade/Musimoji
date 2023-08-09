using System;
using Minis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Musimoji.Scripts
{
    public class MinisNoteInputMapper : MonoBehaviourPlus
    {
        private MidiDevice currentDevice;
        public Action<Note, float> OnNoteDown;
        public Action<Note> OnNoteUp;

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
            OnRemoveMidiDevice();
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change != InputDeviceChange.Added)
            {
                OnRemoveMidiDevice();
                return;
            }

            if (device is not MidiDevice midiDevice)
            {
                OnRemoveMidiDevice();
                return;
            }

            OnAddMidiDevice(midiDevice);
        }

        private void OnAddMidiDevice(MidiDevice midiDevice)
        {
            midiDevice.onWillNoteOn += OnWillNoteOn;
            midiDevice.onWillNoteOff += OnWillNoteOff;
            currentDevice = midiDevice;
        }

        private void OnRemoveMidiDevice()
        {
            if (currentDevice == null) return;
            currentDevice.onWillNoteOn -= OnWillNoteOn;
            currentDevice.onWillNoteOff -= OnWillNoteOff;
            currentDevice = null;
        }

        private void OnWillNoteOn(MidiNoteControl note, float velocity)
        {
            // Note that you can't use note.velocity because the state
            // hasn't been updated yet (as this is "will" event). The note
            // object is only useful to specify the target note (note
            // number, channel number, device name, etc.) Use the velocity
            // argument as an input note velocity.
            if(DebugMessages)Debug.Log(string.Format(
                "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                note.noteNumber,
                note.shortDisplayName,
                velocity,
                (note.device as Minis.MidiDevice)?.channel,
                note.device.description.product
            ));
            OnNoteDown?.Invoke((Note)note.noteNumber, velocity);
        }

        private void OnWillNoteOff(MidiNoteControl note)
        {
            if(DebugMessages)Debug.Log(string.Format(
                "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                note.noteNumber,
                note.shortDisplayName,
                (note.device as Minis.MidiDevice)?.channel,
                note.device.description.product
            ));
            OnNoteUp?.Invoke((Note)note.noteNumber);
        }
    }
}