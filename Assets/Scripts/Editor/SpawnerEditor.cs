using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
[CanEditMultipleObjects]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Show "Script" at the top
        EditorGUI.BeginDisabledGroup(true);
        SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(scriptProp, true, new GUILayoutOption[0]);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("keepParent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isLastOfWave"));
        SerializedProperty spawns = serializedObject.FindProperty("spawns");
        EditorGUILayout.Space();

        if (GUILayout.Button("+"))
        {
            spawns.InsertArrayElementAtIndex(spawns.arraySize);
        }

        for (int i = 0; i < spawns.arraySize; i++)
        {
            SerializedProperty prop = spawns.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Element " + i, GUILayout.ExpandWidth(false), GUILayout.Width(100));
            EditorGUILayout.ObjectField(prop.FindPropertyRelative("objectToSpawn"), GUIContent.none, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("-"))
            {
                spawns.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Offset", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("offset"), GUIContent.none);
            EditorGUILayout.LabelField("Speed", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("speed"), GUIContent.none);
            EditorGUILayout.LabelField("Type", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("typeOverride"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Count", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("count"), GUIContent.none);
            EditorGUILayout.LabelField("CountMax", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("countMax"), GUIContent.none);
            EditorGUILayout.LabelField("Stop", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("stopCountIf"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Delay", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("startDelay"), GUIContent.none);
            EditorGUILayout.LabelField("Interval", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("interval"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("oDelta", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("offsetDelta"), GUIContent.none);
            EditorGUILayout.LabelField("vDelta", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("speedVelocityDelta"), GUIContent.none);
            EditorGUILayout.LabelField("aDelta", GUILayout.ExpandWidth(false), GUILayout.Width(50));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("speedAngleDelta"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("customData"), GUIContent.none);
            EditorGUILayout.Space(24);
        }

        serializedObject.ApplyModifiedProperties();
    }
}