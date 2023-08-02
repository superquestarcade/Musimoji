using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourPlus : MonoBehaviour
{
    public DebugMessageLevel DebugLevel;
    public bool DebugMessages => DebugLevel > 0;
    public enum DebugMessageLevel
    {
        NONE,
        MINIMAL,
        MODERATE,
        ALL
    }
}
