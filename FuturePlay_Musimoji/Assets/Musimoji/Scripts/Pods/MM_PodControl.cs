using System;
using System.Collections;
using System.Collections.Generic;
using Musimoji;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MM_PodControl : MonoBehaviourPlus
{
    [SerializeField] private MusimojiManager manager;
    [SerializeField] private MM_Sequencer sequencer;
    [SerializeField] private PodIndicatorStepData[] indicators;
    [SerializeField] private PodsPhotonParticleWebRequest particleWebRequest;
    private int PodDeviceCount => particleDeviceData.photonDeviceData.Length;
    private Coroutine podCoroutine;

    [Header("Settings")] 
    [SerializeField] private ParticleDeviceDataSo particleDeviceData;
    [SerializeField] private bool indicatorsEnabled = false, debugIndicators;
    [SerializeField] private bool podsEnabled = true, debugPods;
    // [SerializeField] private bool podAudioEnabled = true;
    [SerializeField] private float podLightUpdateDelayPlaymode = 3f, podLightUpdateDelayWinmode = 1f;

    #region Unity Functions
    private void OnEnable()
    {
        manager.sequencer.OnAnyStep.AddListener(OnStep);
        SetIndicatorsEnabled(indicatorsEnabled);
        if (podsEnabled)
        {
            manager.OnWinning.AddListener(StartWinPods);
            StartCoroutine(ResetPodsDelayed(StartUpdatePods));
        }
    }

    private void OnDisable()
    {
        manager.sequencer.OnAnyStep.RemoveListener(OnStep);
        // ResetPods();
        StopAllCoroutines();
    }

    private void OnStep(int step, int stepValue)
    {
        if (indicatorsEnabled) UpdateIndicatorsFromRelativeSequence();
        // if (podsEnabled) UpdatePodsFromRelativeSequence();
    }
    #endregion
    
    #region Indicators
    private void UpdateIndicatorsFromRelativeSequence()
    {
        if(DebugLevel>DebugMessageLevel.MINIMAL && debugIndicators) Debug.Log("MM_PodControl.UpdateIndicatorsFromRelativeSequence");
        for (var i = 0; i < indicators.Length; i++)
        {
            var indicator = indicators[i];
            var podStepValue = manager.GetPodStepValue(indicator.step);
            if(DebugLevel>DebugMessageLevel.MINIMAL && debugIndicators) Debug.Log("MM_PodControl.UpdateIndicatorsFromRelativeSequence" +
                                                               $"indicator {i}, step {indicator.step}, value {podStepValue}");
            indicator.display.SetEmoji(podStepValue);
        }
    }

    public void SetIndicatorEmoji(int step, int value)
    {
        if (!indicatorsEnabled) return;
        for (var index = 0; index < indicators.Length; index++)
        {
            var ind = indicators[index];
            if (ind.step != index) continue;
            ind.display.SetEmoji(value);
        }
    }

    public void ResetIndicators()
    {
        if (!indicatorsEnabled) return;
        foreach(var ind in indicators) ind.display.SetEmoji(0);
    }

    private void SetIndicatorsEnabled(bool value)
    {
        foreach(var ind in indicators) ind.display.gameObject.SetActive(value);
    }
    #endregion
    
    #region Web Pods
    private void UpdatePodsFromRelativeSequence()
    {
        if(DebugLevel>DebugMessageLevel.MINIMAL && debugPods) Debug.Log("MM_PodControl.UpdatePodsFromRelativeSequence");
        for (var i = 0; i < PodDeviceCount; i++)
        {
            var podStepValue = manager.GetPodStepValue(i);
            var deviceData = new ParticleDeviceData();
            particleDeviceData.GetDeviceData(i, ref deviceData);
            var particleColorString = Enum.GetName(typeof(PhotonParticleEmojiString), podStepValue);
            var testString = (podStepValue == 0 ? "off": "yellow");
            if(DebugLevel>DebugMessageLevel.MINIMAL && debugPods) Debug.Log("MM_PodControl.UpdatePodsFromRelativeSequence" +
                                                                            $" pod {i}, value {podStepValue}, colorData {testString}");
            particleWebRequest.SendRequestLightColor(deviceData.deviceID,testString);
        }
    }

    private void StartUpdatePods()
    {
        if(podCoroutine!=null) StopCoroutine(podCoroutine);
        podCoroutine = StartCoroutine(UpdateRandomPodFromSequence(podLightUpdateDelayPlaymode));
    }

    private void StartWinPods(int winningEmojiId)
    {
        if(podCoroutine!=null) StopCoroutine(podCoroutine);
        podCoroutine = StartCoroutine(winningEmojiId>0 ? WinningPodsUpdate(winningEmojiId, podLightUpdateDelayPlaymode) : 
            UpdateRandomPodFromSequence(podLightUpdateDelayPlaymode));
    }

    private IEnumerator UpdateRandomPodFromSequence(float delay)
    {
        while (true)
        {
            var randomPodIndex = Random.Range(0, PodDeviceCount);
            Debug.Assert(sequencer!=null);
            if (sequencer.CurrentSequence == null) yield return new WaitForNextFrameUnit();
            var randomSequenceIndex = Random.Range(0, sequencer.CurrentSequence.Length);
            var sequenceIndex = sequencer.CurrentSequence[randomSequenceIndex];
            var deviceData = new ParticleDeviceData();
            particleDeviceData.GetDeviceData(randomPodIndex, ref deviceData);
            var particleColorString = Enum.GetName(typeof(PhotonParticleEmojiString), sequenceIndex);
            if(DebugLevel>DebugMessageLevel.MINIMAL && debugPods) 
                Debug.Log($"MM_PodControl.UpdateRandomPodFromSequence " +
                          $"podIndex {randomPodIndex}, color {particleColorString}");
            particleWebRequest.SendRequestLightColor(deviceData.deviceID,particleColorString);
            // Only play sound if sequenceIndex>0 as 0=off
            /*if (podAudioEnabled && sequenceIndex>0)
            {
                // var particleSoundString = Enum.GetName(typeof(PhotonParticleAudioString), sequenceIndex);
                yield return new WaitForSeconds(0.01f);
                if(DebugLevel>DebugMessageLevel.MINIMAL && debugPods) 
                    Debug.Log($"MM_PodControl.UpdateRandomPodFromSequence " +
                              $"podIndex {randomPodIndex}, sound {sequenceIndex}");
                particleWebRequest.SendRequestAudio(deviceData.deviceID,sequenceIndex.ToString());
            }*/
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator WinningPodsUpdate(int winIndex, float delay)
    {
        var podIds = new List<int>().BuildIncrementedIntList(PodDeviceCount);
        var particleColorString = Enum.GetName(typeof(PhotonParticleEmojiString), winIndex);
        while (podIds.Count > 0)
        {
            var randomPodIndex = Random.Range(0, podIds.Count);
            var deviceData = new ParticleDeviceData();
            particleDeviceData.GetDeviceData(randomPodIndex, ref deviceData);
            if(DebugLevel>DebugMessageLevel.MINIMAL && debugPods) 
                Debug.Log($"MM_PodControl.WinningPodsUpdate " +
                          $"podIndex {randomPodIndex}, color {particleColorString}");
            particleWebRequest.SendRequestLightColor(deviceData.deviceID,particleColorString);
            podIds.Remove(randomPodIndex);
            yield return new WaitForSeconds(delay);
        }
    }

    public void ResetPods()
    {
        for (var i = 0; i < PodDeviceCount; i++)
        {
            var deviceData = new ParticleDeviceData();
            particleDeviceData.GetDeviceData(i, ref deviceData);
            var particleColorString = Enum.GetName(typeof(PhotonParticleEmojiString), 0);
            particleWebRequest.SendRequestLightColor(deviceData.deviceID,particleColorString);
        }
    }

    private IEnumerator ResetPodsDelayed(Action callback = null)
    {
        var count = 0;
        while (count < PodDeviceCount)
        {
            var deviceData = new ParticleDeviceData();
            particleDeviceData.GetDeviceData(count, ref deviceData);
            particleWebRequest.SendRequestLightColor(deviceData.deviceID,"off");
            count++;
            yield return new WaitForSeconds(0.1f);
        }
        callback?.Invoke();
    }
    #endregion
    
    #region Data
    [Serializable]
    public class PodIndicatorStepData
    {
        public int step;
        public MusimojiSequenceStepDisplay display;
    }
    #endregion
}
