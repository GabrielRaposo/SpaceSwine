using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools
{
    public static bool SettableButton(string name,bool enabled)
    {
        bool b;
        
        GUI.enabled = enabled;
        b = GUILayout.Button(name);
        GUI.enabled = true;

        return b;
    }
}
