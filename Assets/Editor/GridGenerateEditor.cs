using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator generator = (GridGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Grid"))
        {
            generator.GenerateGridInEditor();
        }
    }
}
