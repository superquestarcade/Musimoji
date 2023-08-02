using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoadSceneButton), true)]
public class LoadSceneButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var picker = target as LoadSceneButton;
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.sceneToLoad);

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newScene = EditorGUILayout.ObjectField("scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            var scenePathProperty = serializedObject.FindProperty("sceneToLoad");
            scenePathProperty.stringValue = newPath;
        }
        serializedObject.ApplyModifiedProperties();
    }
}