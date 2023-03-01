using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParseInputTag))]
public class ParseInputTextEditor : LocalizedTextEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        ParseInputTag obj = target as ParseInputTag;
        obj.separator = EditorGUILayout.TextField("Separator", obj.separator);
    }
}
