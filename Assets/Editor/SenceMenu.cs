using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

public static class SenceMenu
{
    [MenuItem("OpenScene/Garage %&-")]
    private static void OpenGameScene()
    {
        OpenScene("Assets/Scenes/Final/Garage.unity");
    }

    [MenuItem("OpenScene/LevelEditor %&1")]
    private static void OpenLevelEditor()
    {
        OpenScene("Assets/Scenes/Final/Desert_Circuit_Night.unity");
    }
    //[MenuItem("OpenScene/Loading %&=")]
    //private static void OpenLoadingScene()
    //{
    //    OpenScene("Assets/_GameAssets/Scenes/Loading.unity");
    //}
    //[MenuItem("OpenScene/MeshEditor %&2")]
    //private static void OpenMeshEditor()
    //{
    //    OpenScene("Assets/_GameAssets/Scenes/MeshEditor.unity");
    //}

    private static void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}