using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenameRoundsByOrder))]
public class RenameRoundsByOrderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RenameRoundsByOrder myScript = (RenameRoundsByOrder) target;
        if(GUILayout.Button("Update Names"))
        {
            myScript.UpdateNames();
        }

        DrawDefaultInspector();
    }
}
