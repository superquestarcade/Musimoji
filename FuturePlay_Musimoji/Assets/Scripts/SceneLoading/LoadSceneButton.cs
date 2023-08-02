using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    private AudioSource menuAudio;

    public string sceneToLoad;

    public void Update()
    {
        menuAudio = GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<AudioSource>();
    }

    public void OnClick()
    {
        SceneLoadManager.singleton.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        this.gameObject.GetComponent<Button>().interactable = false;
        menuAudio.Stop();
    }
}

