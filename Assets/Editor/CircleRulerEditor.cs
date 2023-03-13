using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleRuler)), CanEditMultipleObjects]
public class CircleRulerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = (CircleRuler)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Get children"))
            obj.GetAllChildren();

        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            var child = obj.objects[0];
            var newChild = Instantiate(child, child.parent);
            obj.objects.Add(newChild);
            obj.OnValidate();
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        base.OnInspectorGUI();
    }
}
