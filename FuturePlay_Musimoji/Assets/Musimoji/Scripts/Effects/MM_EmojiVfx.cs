using System;
using UnityEngine;
using UnityEngine.VFX;

public class MM_EmojiVfx : MonoBehaviour
{
    public EmojiParticleGroup[] emojiParticleGroups;


    public void TriggerGroup(int emojiIndex)
    {
        emojiParticleGroups[emojiIndex].Trigger();
    }
}

[Serializable]
public class EmojiParticleGroup
{
    public ParticleSystem[] particles;

    public void Trigger()
    {
        foreach(var v in particles) v.Play();
    }
}
