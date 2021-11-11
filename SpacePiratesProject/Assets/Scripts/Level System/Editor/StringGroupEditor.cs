using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StatusManager.StringGroup))]
public class StringGroupEditor : PropertyDrawer
{
    private SerializedProperty strings;
    private SerializedProperty dialogueEvent;
    private bool cache = false;



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            property.Next(true);
            strings = property.Copy();
            property.Next(false);
            dialogueEvent = property.Copy();
            cache = true;
        }

        EditorGUI.PropertyField(position, strings, label, true);
        position.y += EditorGUI.GetPropertyHeight(strings, label, true);
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(position, dialogueEvent);
        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (cache)
        {
            return EditorGUI.GetPropertyHeight(strings, label) + EditorGUI.GetPropertyHeight(dialogueEvent, label) + 2;
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
