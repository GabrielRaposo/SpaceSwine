using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private bool baseToggle;
    
    public override void OnInspectorGUI()
    {
        // baseToggle = GUILayout.Toggle(baseToggle, "", EditorStyles.foldoutPreDrop);
         //if(baseToggle)
         
        //base.OnInspectorGUI();
        
        GUILayout.Space(20);
            

        LocalizedText obj = target as LocalizedText;

        obj.textMesh = (TMP_Text)EditorGUILayout.ObjectField("TextMesh",obj.textMesh, typeof(TMP_Text));

        if (obj.textMesh == null)
        {
            EditorGUILayout.HelpBox("TextComponent not found!", MessageType.Error);
            return;
        }

        obj.textType =(LocalizedTextTypes)EditorGUILayout.EnumPopup("Text type", obj.textType);

        SetLocalizationCodeEnum(obj);

        GUILayout.Space(18);
        
        obj.fallbackText = EditorGUILayout.TextField("Fallback Text", obj.fallbackText);
        
        EditorUtility.SetDirty(obj);
    }

    private void SetLocalizationCodeEnum(LocalizedText localizedText)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Localized Text Code",GUILayout.ExpandWidth(false));
        
        if (localizedText.editor_manualCode)
        {
            localizedText.localizationCode = GUILayout.TextArea(localizedText.localizationCode,GUILayout.ExpandWidth(true));
        }
        else
        {
            List<string> displayList = new List<string>();
            List<string> codeList = new List<string>();
            Dictionary<string, LanguageToString>.KeyCollection keys;

            switch (localizedText.textType)
            {
                case LocalizedTextTypes.UI:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.UI).dic.Keys;

                    foreach (string key in keys)
                    {
                        string preview = LocalizationManager.GetUiText(key, "[not found]");
                        if (preview.Length > 20)
                        {
                            preview = preview.Remove(19, preview.Length - 20);
                            preview += "...";
                        }
                        
                        displayList.Add($"{key} - {preview}");
                        codeList.Add(key);
                    }
                    break;
                
                case LocalizedTextTypes.Story:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.Story).dic.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {LocalizationManager.GetStoryText(key)}");
                        codeList.Add(key);
                    }
                        
                    
                    break;
                
                case LocalizedTextTypes.Inputs:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.Inputs).dic.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {LocalizationManager.GetInputText(key)}");
                        codeList.Add(key);
                    }
                    
                    break;
                
                case LocalizedTextTypes.Achievement:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.Achievement).dic.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {LocalizationManager.GetAchievementName(key)}");
                        codeList.Add(key);
                    }
                        
                    
                    break;
                case LocalizedTextTypes.Music:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.Music).dic.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {LocalizationManager.GetMusicText(key)}");
                        codeList.Add(key);
                    }
                    
                    
                    break;
                
                case LocalizedTextTypes.Nave:
                    keys = LocalizationManager.LocalizationFile(LocalizedTextTypes.Nave).dic.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {LocalizationManager.GetShipText(key)}");
                        codeList.Add(key);
                    }
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            int index = codeList.FindIndex((x) => x == localizedText.localizationCode);
            if (index == -1)
                index = 0;

            localizedText.localizationCode = codeList[EditorGUILayout.Popup(index, displayList.ToArray())];

        }
        localizedText.editor_manualCode = GUILayout.Toggle(localizedText.editor_manualCode,"", GUILayout.MaxWidth(18));
        EditorGUILayout.EndHorizontal();
        
    }
}
