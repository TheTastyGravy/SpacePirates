#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicData))]
public class MusicDataEditor : Editor
{
    SerializedProperty musicData;
    SerializedProperty addFlag;
    SerializedProperty printFlag;



    void OnEnable()
    {
        musicData = serializedObject.FindProperty("musicData");
        addFlag = serializedObject.FindProperty("addFlag");
        printFlag = serializedObject.FindProperty("printFlag");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int count = musicData.arraySize;
        for (int i = 0; i < count; i++)
		{
            SerializedProperty propertyIter = musicData.GetArrayElementAtIndex(i);
            // Display the name in a text field so it can be edited
            propertyIter.NextVisible(true);
            propertyIter.stringValue = EditorGUILayout.TextField(propertyIter.stringValue);
            EditorGUI.indentLevel += 2;

            propertyIter.NextVisible(false);
            // Display scenes as an enum flag (multiple selection)
            propertyIter.intValue = (int)(GameManager.GameState)EditorGUILayout.EnumFlagsField(propertyIter.displayName, (GameManager.GameState)propertyIter.intValue);
            // Draw other properties as normal
            for (int j = 0; j < 2; j++)
			{
                propertyIter.NextVisible(false);
                EditorGUILayout.PropertyField(propertyIter);
            }

            propertyIter.NextVisible(false);
            // Draw button to remove an element
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            propertyIter.boolValue = GUILayout.Button("Remove Element", GUILayout.Width(150), GUILayout.Height(25));
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.Space(10);
		}

        // Draw button to add new element and print how they will be used
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        addFlag.boolValue = GUILayout.Button("Add", GUILayout.Width(100), GUILayout.Height(25));
        GUILayout.Space(25);
        printFlag.boolValue = GUILayout.Button("Print", GUILayout.Width(100), GUILayout.Height(25));
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif