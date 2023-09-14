using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Musimoji
{
    [CreateAssetMenu(fileName = "PodColorData", menuName = "Musimoji/PodColorData")]
    public class MM_PodColorData : ScriptableObject
    {
        public PodColorData[] data;
    }

    [Serializable]
    public class PodColorData
    {
        public PhotonParticleEmojiString emojiString;
        // public PhotonParticleAudioString audioString;
        public string webCommandString;
    }

    [Serializable]
    public enum PhotonParticleEmojiString
    {
        off,
        joy,
        trust,
        fear,
        surprise,
        sadness,
        disgust,
        anger,
        anticipation,
    }

    /*[Serializable]
    public enum PhotonParticleAudioString
    {
        off,
        joy,
        trust,
        fear,
        surprise,
        sadness,
        disgust,
        anger,
        anticipation,
    }*/
}