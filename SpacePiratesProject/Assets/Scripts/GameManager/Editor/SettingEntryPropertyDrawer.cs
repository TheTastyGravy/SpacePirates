using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LevelDificultyData.BaseClass), true)]
public class SettingEntryPropertyDrawer : PropertyDrawer
{
    private SerializedProperty playerCountValues;
    private SerializedProperty useSeperateValues;
    private bool cache = false;



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            property.Next(true);
            playerCountValues = property.Copy();
            property.Next(false);
            useSeperateValues = property.Copy();
            cache = true;
        }
        // Force the currect array size
        playerCountValues.arraySize = 4;

        if (useSeperateValues.boolValue)
		{
            position.height = EditorGUI.GetPropertyHeight(playerCountValues, label, true);
            EditorGUI.PropertyField(position, playerCountValues, label, true);
            // Move down and reset height
            position.y += position.height;
            position.height = EditorGUIUtility.singleLineHeight;
        }
		else
		{
            // Make property to draw using 1 line instead of the whole area
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, playerCountValues.GetArrayElementAtIndex(0), label, false);
            // Move down
            position.y += EditorGUI.GetPropertyHeight(playerCountValues.GetArrayElementAtIndex(0), label, false);
        }

        EditorGUI.indentLevel += 2;
        EditorGUI.PropertyField(position, useSeperateValues);
        EditorGUI.indentLevel -= 2;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (cache)
		{
            float baseHeight = useSeperateValues.boolValue ? EditorGUI.GetPropertyHeight(playerCountValues, label, true) : EditorGUI.GetPropertyHeight(playerCountValues.GetArrayElementAtIndex(0), label, false);
            return baseHeight + EditorGUI.GetPropertyHeight(useSeperateValues);
        }
        return EditorGUI.GetPropertyHeight(property);
    }
}
