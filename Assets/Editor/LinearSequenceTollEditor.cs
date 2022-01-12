using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LinearSequenceTool))]
public class LinearSequenceTollEditor : Editor
{
    protected void OnSceneGUI()
    {
        LinearSequenceTool obj = (LinearSequenceTool)target;

        if (obj.objects == null) obj.objects = new List<ObjectPoint>();

        foreach (var o in obj.turningPoints)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(o.position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "");
                o.position = newPos;
                obj.OnValidate();
            }
        }

    }
}
