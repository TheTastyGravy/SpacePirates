using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StatusManager.StringGroup))]
public class StringGroupEditor : PropertyDrawer
{
    private SerializedProperty strings;
    private bool cache = false;



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            property.Next(true);
            strings = property.Copy();
            cache = true;
        }

        EditorGUI.PropertyField(position, strings, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(strings ?? property, label) ;
    }
}
