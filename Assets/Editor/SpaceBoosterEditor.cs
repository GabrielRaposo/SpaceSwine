using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpaceBooster))]
public class SpaceBoosterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var obj = (SpaceBooster)target;
        
        GUILayout.Space(16);
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        
        //⬅️➡️↙️↘️⬇️
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("↖"))
            obj.SetFirstLaunchDirection(-1,1);
            
        if(GUILayout.Button("⬆"))
            obj.SetFirstLaunchDirection(0,1);
        
        if(GUILayout.Button("↗"))
            obj.SetFirstLaunchDirection(1,1);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("<-"))
            obj.SetFirstLaunchDirection(-1,0);
            
        if(GUILayout.Button(""))
            obj.SetFirstLaunchDirection(0,0);
        
        if(GUILayout.Button("➡"))
            obj.SetFirstLaunchDirection(1,0);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("↙"))
            obj.SetFirstLaunchDirection(-1,-1);
            
        if(GUILayout.Button("v"))
            obj.SetFirstLaunchDirection(0,-1);

        if (GUILayout.Button("↘"))
            obj.SetFirstLaunchDirection(1,-1);
            
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        EditorUtility.SetDirty(obj);
        
        
    }
}
