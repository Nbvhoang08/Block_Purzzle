using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]

public class ShapeDataDrawer : Editor
{
    private ShapeData shapeDataInstance => target as ShapeData;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumnInputFields();
        EditorGUILayout.Space();

        if (shapeDataInstance.board != null && shapeDataInstance.columns > 0 && shapeDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(shapeDataInstance);
        }
    }
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            shapeDataInstance.Clear();
        }
    }
    private void DrawColumnInputFields()
    {
        var columnTemp = shapeDataInstance.columns;
        var rowTemp = shapeDataInstance.rows;
        shapeDataInstance.columns = EditorGUILayout.IntField("Columns", shapeDataInstance.columns);
        shapeDataInstance.rows = EditorGUILayout.IntField("Rows", shapeDataInstance.rows);
        if ((shapeDataInstance.columns != columnTemp || shapeDataInstance.rows != rowTemp)&& shapeDataInstance.columns> 0 &&shapeDataInstance.rows > 0) 
        {
            shapeDataInstance.CreateNewBoard();
        }
    }
    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);    
        tableStyle.margin.left =  32;
        
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;
        
        var RowStyle = new GUIStyle();
        RowStyle.fixedHeight = 25;
        RowStyle.alignment = TextAnchor.MiddleCenter;
        
        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for(var row = 0; row < shapeDataInstance.rows; row++) 
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);
            for (var col = 0; col < shapeDataInstance.columns; col++)
            {
                EditorGUILayout.BeginHorizontal(RowStyle);
                var data = EditorGUILayout.Toggle(shapeDataInstance.board[row].column[col], dataFieldStyle);
                shapeDataInstance.board[row].column[col] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
