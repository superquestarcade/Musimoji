using UnityEngine;

namespace Musimoji
{
    [CreateAssetMenu(fileName = "ParticleData_Data", menuName = "Photon Particle System/ParticleData")]
    public class ParticleDeviceData : ScriptableObject
    {
        #region Public Variables
        [Space, Header("Photon Particle Devices")]
        public string deviceName = default;
        public string deviceID = default;
        #endregion
    }
}
