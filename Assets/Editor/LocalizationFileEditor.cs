using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LocalizationEditor : EditorWindow
{
    [MenuItem("Window/Localization Updates")] 
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(LocalizationEditor));
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("UI"))
        {
            LocalizationManager.CallUpdate(LocalizedTextTypes.UI);
            EditorUtility.SetDirty(LocalizationManager.UiFile);
        }

        if (GUILayout.Button("Story"))
        {
            LocalizationManager.CallUpdate(LocalizedTextTypes.Story);
            EditorUtility.SetDirty(LocalizationManager.StoryFile);
        }

        if (GUILayout.Button("Achievements"))
        {
            LocalizationManager.CallUpdate(LocalizedTextTypes.Achievement);
            EditorUtility.SetDirty(LocalizationManager.AchievementsFile);
        }

        if (GUILayout.Button("Music"))
        {
            LocalizationManager.CallUpdate(LocalizedTextTypes.Music);   
            EditorUtility.SetDirty(LocalizationManager.MusicFile);
        }
            
    }
}
