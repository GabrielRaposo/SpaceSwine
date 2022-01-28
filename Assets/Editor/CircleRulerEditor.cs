using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleRuler))]
public class CircleRulerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = (CircleRuler)target;

        if (GUILayout.Button("Get children"))
            obj.GetAllChildren();
        
        EditorGUILayout.Space(10);
        
        base.OnInspectorGUI();
    }
}
