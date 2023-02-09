using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;


public class LocalizationEditor : EditorWindow
{
    [MenuItem("Window/Localization Updates")]
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(LocalizationEditor));
    }
    
    private bool locked;
    
    private void OnEnable()
    {
        locked = false;
    }

    private void OnGUI()
    {
        
        if (locked)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Loading files...\n\nLembra de salvar depois");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Force editor reset"))
                locked = false;
            
            return;
        }
        
        if (GUILayout.Button("UI"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.UI0, ()=>SetDirtyAndUnlock(LocalizationManager.UiFile));
        }

        if (GUILayout.Button("Inputs"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.Inputs, () => SetDirtyAndUnlock(LocalizationManager.InputFile));
        }
        
        if (GUILayout.Button("Story"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.Story, ()=>SetDirtyAndUnlock(LocalizationManager.StoryFile));
        }

        if (GUILayout.Button("Achievements"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.Achievement, ()=>SetDirtyAndUnlock(LocalizationManager.AchievementsFile));
            //EditorUtility.SetDirty(LocalizationManager.AchievementsFile);
        }

        if (GUILayout.Button("Music"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.Music, ()=>SetDirtyAndUnlock(LocalizationManager.MusicFile));   
            //EditorUtility.SetDirty(LocalizationManager.MusicFile);
        }

        if (GUILayout.Button("Nave"))
        {
            locked = true;
            LocalizationManager.CallUpdate(LocalizedTextTypes.Nave, ()=>SetDirtyAndUnlock(LocalizationManager.NaveFile));
            //EditorUtility.SetDirty(LocalizationManager.NaveFile);
        }
            
    }
    
    private void SetDirtyAndUnlock(AstroPigLocalizationFile file)
    {
        locked = false;
        EditorUtility.SetDirty(file);
        Repaint();
    }
    
}
