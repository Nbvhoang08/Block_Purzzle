using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public class MissingScriptCleaner : EditorWindow
{
    private string folderPath = "Assets";

    [MenuItem("Tools/Cleanup/Missing Script Cleaner")]
    public static void ShowWindow()
    {
        var window = GetWindow<MissingScriptCleaner>("Missing Script Cleaner");
        window.minSize = new Vector2(450, 250);
    }

    private void OnGUI()
    {
        GUILayout.Label("🧹 Remove All Missing Scripts", EditorStyles.boldLabel);
        GUILayout.Space(5f);

        GUILayout.Label("📁 Target Folder for Prefabs:", EditorStyles.label);
        GUILayout.BeginHorizontal();
        folderPath = GUILayout.TextField(folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                    folderPath = "Assets" + selected.Substring(Application.dataPath.Length);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        if (GUILayout.Button("🧨 Remove Missing Scripts in Prefabs", GUILayout.Height(40)))
        {
            RemoveMissingScriptsInPrefabs(folderPath);
        }

        GUILayout.Space(10f);

        if (GUILayout.Button("🚿 Remove Missing Scripts in Current Scene", GUILayout.Height(40)))
        {
            RemoveMissingScriptsInCurrentScene();
        }
    }

    private static void RemoveMissingScriptsInPrefabs(string folderPath)
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        int totalRemoved = 0;
        int totalPrefabs = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            if (prefabRoot == null)
                continue;

            int removed = RemoveMissingScriptsRecursive(prefabRoot);
            if (removed > 0)
            {
                EditorUtility.SetDirty(prefabRoot);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                Debug.Log($"✅ Removed {removed} missing script(s) from: {path}", prefabRoot);
                totalRemoved += removed;
                totalPrefabs++;
            }

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        EditorUtility.DisplayDialog("Missing Script Cleaner",
            $"✅ DONE (Prefabs)!\n\n🧱 Prefabs cleaned: {totalPrefabs}\n💀 Missing Scripts Removed: {totalRemoved}",
            "OK");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RemoveMissingScriptsInCurrentScene()
    {
        // ✅ Dùng API mới: FindObjectsByType (Unity 2023+)
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        int totalRemoved = 0;
        int totalObjects = 0;

        foreach (GameObject go in allObjects)
        {
            int removed = RemoveMissingScriptsRecursive(go);
            if (removed > 0)
            {
                Undo.RegisterCompleteObjectUndo(go, "Remove missing script (Scene)");
                EditorUtility.SetDirty(go);
                totalRemoved += removed;
                totalObjects++;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Scene Cleanup Complete",
            $"✅ DONE (Scene)!\n\n🎭 Objects cleaned: {totalObjects}\n💀 Missing Scripts Removed: {totalRemoved}",
            "OK");
    }

    private static int RemoveMissingScriptsRecursive(GameObject go)
    {
        int removedCount = 0;

        var components = go.GetComponents<MonoBehaviour>();
        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (components[i] == null)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                removedCount++;
            }
        }

        foreach (Transform child in go.transform)
        {
            removedCount += RemoveMissingScriptsRecursive(child.gameObject);
        }

        return removedCount;
    }
}
