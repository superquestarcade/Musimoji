using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PodsPhotonParticleWebRequest : MonoBehaviourPlus
{
    #region Properties
    [SerializeField] private string accessToken = "35dc9ce0688eaed0a51d5fa45de9894046caa98f";
    private const string _particleIoStartURL = "https://api.particle.io/v1/devices/";
    private const string _particleIoSFXEndURL = "/playWav?access_token=";
    private const string _particleIoLightEndURL = "/lights?access_token=";
    private const string _particleIoAttractLightEndURL = "/attract?access_token=";

    private Coroutine lightWebRequestCoroutine, soundWebRequestCoroutine;
    private UnityWebRequest lightWebRequest, soundWebRequest;
    
    #endregion
    
    #region Requests

    public void SendRequestLightColor(string deviceId, string light)
    {
        if (!isActiveAndEnabled) return;
        if (lightWebRequestCoroutine != null)
        {
            lightWebRequest?.Dispose();
            StopCoroutine(lightWebRequestCoroutine);
        }
        lightWebRequestCoroutine = StartCoroutine(SendRequestLightColorToPhoton(deviceId, light));
    }

    public void SendRequestAudio(string deviceId, string audio)
    {
        if (!isActiveAndEnabled) return;
        if (soundWebRequestCoroutine != null)
        {
            soundWebRequest?.Dispose();
            StopCoroutine(soundWebRequestCoroutine);
        }
        soundWebRequestCoroutine = StartCoroutine(SendRequestMusicToPhoton(deviceId, audio));
    }
    
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine to Send for request from Unity Button to Photonboard to change Light;
    /// </summary>
    /// <returns> Float Delay; </returns>
    private IEnumerator SendRequestLightColorToPhoton(string deviceId, string light)
    {
        var form = new WWWForm();
        var url = $"{_particleIoStartURL}{deviceId}{_particleIoLightEndURL}{accessToken}";
        form.AddField("args", light);
        if(DebugMessages) Debug.Log($"PodsPhotonParticleWebRequest.SendRequestLightColorToPhoton deviceId {deviceId}, light {light}, url {url}, form.data.Length {form.data.Length}");

        lightWebRequest = UnityWebRequest.Post(url, form);
        yield return lightWebRequest.SendWebRequest();

        if (lightWebRequest.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
            Debug.Log(lightWebRequest.error);
        if(DebugMessages) Debug.Log($"PodsPhotonParticleWebRequest.SendRequestLightColorToPhoton result {lightWebRequest.result}");
        lightWebRequest.Dispose();
    }

    /// <summary>
    /// Coroutine to Send for request from Unity Button to Photonboard to change SFX;
    /// </summary>
    /// <returns> Float Delay; </returns>
    private IEnumerator SendRequestMusicToPhoton(string deviceId, string audio)
    {
        var form = new WWWForm();
        var url = $"{_particleIoStartURL}{deviceId}{_particleIoSFXEndURL}{accessToken}";
        form.AddField("args", audio + ".wav");

        soundWebRequest = UnityWebRequest.Post(url, form);
        yield return soundWebRequest.SendWebRequest();

        if (soundWebRequest.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
            Debug.Log(soundWebRequest.error);
        if(DebugMessages) Debug.Log($"PodsPhotonParticleWebRequest.SendRequestMusicToPhoton result {soundWebRequest.result}");
        soundWebRequest.Dispose();
    }
    #endregion
}