using System.Collections;
using System.Collections.Generic;
using Musimoji;
using UnityEngine;
using UnityEngine.Networking;

public class PodsPhotonParticleWebRequest : MonoBehaviour
{
    #region Properties
    [SerializeField]
    [Tooltip("Photon Particle ScriptableObjects")]
    private List<ParticleDeviceData> particlesData = default;
    #endregion

    #region Private Variables
    private const string _particleIoStartURL = "https://api.particle.io/v1/devices/";
    private const string _particleIoSFXEndURL = "/playWav?access_token=35dc9ce0688eaed0a51d5fa45de9894046caa98f";
    private const string _particleIoLightEndURL = "/lights?access_token=35dc9ce0688eaed0a51d5fa45de9894046caa98f";
    private const string _particleIoAttractLightEndURL = "/attract?access_token=35dc9ce0688eaed0a51d5fa45de9894046caa98f";
    public int PodID { get => _currPodID; set => _currPodID = value; }
    [SerializeField] private int _currPodID = default;
    #endregion
    
    #region Requests

    public void SendRequestLightColor(string light)
    {
        StartCoroutine(SendRequestLightColorToPhoton(light));
    }

    public void SendRequestAudio(string audio)
    {
        StartCoroutine(SendRequestMusicToPhoton(audio));
    }
    
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine to Send for request from Unity Button to Photonboard to change Light;
    /// </summary>
    /// <returns> Float Delay; </returns>
    private IEnumerator SendRequestLightColorToPhoton(string light)
    {
        var form = new WWWForm();
        var url = $"{_particleIoStartURL}{particlesData[PodID].deviceID}{_particleIoLightEndURL}";
        form.AddField("args", light);

        var www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
    }

    /// <summary>
    /// Coroutine to Send for request from Unity Button to Photonboard to change SFX;
    /// </summary>
    /// <returns> Float Delay; </returns>
    private IEnumerator SendRequestMusicToPhoton(string sfx)
    {
        var form = new WWWForm();
        var url = $"{_particleIoStartURL}{particlesData[PodID].deviceID}{_particleIoSFXEndURL}";
        form.AddField("args", sfx);

        var www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
    }
    #endregion
}