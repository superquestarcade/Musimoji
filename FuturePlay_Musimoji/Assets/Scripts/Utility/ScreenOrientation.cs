using UnityEngine;

public class ScreenOrientation : MonoBehaviour
{
    public int horizontalRes = 1920, verticalRes = 1080;
    public bool fullscreen = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
        //Set screen size for Standalone
#if UNITY_STANDALONE
        Screen.SetResolution(verticalRes, horizontalRes, false);
        Screen.fullScreen = fullscreen;
#endif
    }
}
