using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitKeyReturnToMainMenu : MonoBehaviour
{
    private void OnCancel()
    {
        Debug.Log("AnyKeyReturnToMainMenu.OnCancel return to main menu");
        
        if (SceneLoadManager.singleton == null) return;
        
        SceneLoadManager.singleton.ReturnToMainMenu();
    }
}
