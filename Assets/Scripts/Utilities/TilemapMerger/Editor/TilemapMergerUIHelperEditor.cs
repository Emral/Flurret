using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapMergerUIHelper))]
public class TilemapMergerUIHelperEditor : TilemapMergerEditor
{
    void OnSceneGUI()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(100, 20, 80, 40));
        if (GUILayout.Button("Save"))
        {
            ((TilemapMergerUIHelper)target).Save();
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}
