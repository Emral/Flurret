using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapMerger))]
public class TilemapMergerEditor : Editor
{
    void OnSceneGUI()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(100, 20, 80, 40));
        if (GUILayout.Button("Save"))
        {
            ((TilemapMerger)target).Save();
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}
