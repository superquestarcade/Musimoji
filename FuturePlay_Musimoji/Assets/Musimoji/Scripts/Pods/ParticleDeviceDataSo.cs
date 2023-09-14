using System;
using UnityEngine;

namespace Musimoji
{
    [CreateAssetMenu(fileName = "ParticleData_Data", menuName = "Photon Particle System/ParticleData")]
    public class ParticleDeviceDataSo : ScriptableObject
    {
        #region Public Variables

        [Space, Header("Photon Particle Devices")]
        public ParticleDeviceData[] photonDeviceData;

        #endregion

        public bool GetDeviceData(int podNumber, ref ParticleDeviceData data)
        {
            if (photonDeviceData.Length <= podNumber)
            {
                Debug.LogError($"ParticleDeviceDataSo.GetDeviceData no device data for podNumber {podNumber}");
                return false;
            }
            data = photonDeviceData[podNumber];
            if (data != null) return true;
            Debug.LogError($"ParticleDeviceDataSo.GetDeviceData null data for podNumber {podNumber}");
            return false;
        }
    }
    
    [Serializable]
    public class ParticleDeviceData
    {
        public string deviceName = default;
        public string deviceID = default;
    }
}
