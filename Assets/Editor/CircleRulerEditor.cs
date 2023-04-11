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

        if (GUILayout.Button("R", GUILayout.Width(30)))
        {
            for (int i = 0; i < obj.objects.Count; i++)
            {
                var t = obj.objects[i];
                t.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            }
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        base.OnInspectorGUI();
    }
}
