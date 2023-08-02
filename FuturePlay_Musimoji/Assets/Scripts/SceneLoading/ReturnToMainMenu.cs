using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMainMenu : MonoBehaviour
{
    public void Return()
    {
        if (SceneLoadManager.singleton == null) return;
        
        Debug.Log("ReturnToMainMenu.Return");
        
        SceneLoadManager.singleton.ReturnToMainMenu();
    }
}
