using System;
using System.Collections.Generic;
using Minis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Musimoji
{
    public class MinisNoteInputMapper : MonoBehaviourPlus
    {
        private List<MidiDevice> currentDevices = new();
        public Action<int, Note, float> OnNoteDown;
        public Action<int, Note> OnNoteUp;

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is not MidiDevice midiDevice) return;
            switch (change)
            {
                case InputDeviceChange.Added:
                    OnAddMidiDevice(midiDevice);
                    break;
                case InputDeviceChange.Removed:
                    OnRemoveMidiDevice(midiDevice);
                    break;
                case InputDeviceChange.Disconnected:
                    break;
                case InputDeviceChange.Reconnected:
                    break;
                case InputDeviceChange.Enabled:
                    break;
                case InputDeviceChange.Disabled:
                    break;
                case InputDeviceChange.UsageChanged:
                    break;
                case InputDeviceChange.ConfigurationChanged:
                    break;
                case InputDeviceChange.SoftReset:
                    break;
                case InputDeviceChange.HardReset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }

        private void OnAddMidiDevice(MidiDevice midiDevice)
        {
            if(DebugMessages)Debug.Log(string.Format(
                // "Adding device ch:{0} player: {1} dev:'{2}'",
                "Adding device ch:{0} dev:'{1}'",
                midiDevice.channel,
                // DeviceChannelToPlayerId(midiDevice.channel),
                midiDevice.description.product
            ));
            midiDevice.onWillNoteOn += OnWillNoteOn;
            midiDevice.onWillNoteOff += OnWillNoteOff;
            currentDevices.Add(midiDevice);
            CheckDevices();
        }

        private void OnRemoveMidiDevice(MidiDevice midiDevice)
        {
            if(DebugMessages)Debug.Log(string.Format(
                // "Removing device ch:{0} player: {1} dev:'{2}'",
                "Removing device ch:{0} dev:'{1}'",
                midiDevice.channel,
                // DeviceChannelToPlayerId(midiDevice.channel),
                midiDevice.description.product
            ));
            midiDevice.onWillNoteOn -= OnWillNoteOn;
            midiDevice.onWillNoteOff -= OnWillNoteOff;
            currentDevices.Remove(midiDevice);
            CheckDevices();
        }

        private void CheckDevices()
        {
            var activeDevices = new List<MidiDevice>();
            foreach (var midiDevice in currentDevices)
            {
                if(midiDevice is {enabled: true}) activeDevices.Add(midiDevice);
            }
            currentDevices = activeDevices;
        }

        private int DeviceChannelToPlayerId(int channel)
        {
            for (var id = 0; id < currentDevices.Count; id++)
            {
                var device = currentDevices[id];
                if (device.channel == channel) return id;
            }
            return -1;
        }

        private void OnWillNoteOn(MidiNoteControl note, float velocity)
        {
            var channel = -1;
            if (note.device is not Minis.MidiDevice midiDevice)
            {
                return;
            }
            channel = midiDevice.channel;
            // var playerId = DeviceChannelToPlayerId(channel);
            // Note that you can't use note.velocity because the state
            // hasn't been updated yet (as this is "will" event). The note
            // object is only useful to specify the target note (note
            // number, channel number, device name, etc.) Use the velocity
            // argument as an input note velocity.
            if(DebugMessages)Debug.Log(string.Format(
                // "Note On #{0} ({1}) vel:{2:0.00} ch:{3} player: {4} dev:'{5}'",
                "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                note.noteNumber,
                note.shortDisplayName,
                velocity,
                channel,
                // playerId,
                note.device.description.product
            ));
            OnNoteDown?.Invoke(channel, (Note)note.noteNumber, velocity);
        }

        private void OnWillNoteOff(MidiNoteControl note)
        {
            var channel = -1;
            if (note.device is not Minis.MidiDevice midiDevice)
            {
                return;
            }
            channel = midiDevice.channel;
            // var playerId = DeviceChannelToPlayerId(channel);
            if(DebugMessages)Debug.Log(string.Format(
                // "Note Off #{0} ({1}) ch:{2} player:{3} dev:'{4}'",
                "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                note.noteNumber,
                note.shortDisplayName,
                channel,
                // playerId,
                note.device.description.product
            ));
            OnNoteUp?.Invoke(channel, (Note)note.noteNumber);
        }
    }
}