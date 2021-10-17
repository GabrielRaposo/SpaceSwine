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
    private AstroPigLocalizationFile localization;
    
    public override void OnInspectorGUI()
    {
        localization = LocalizationManager.LocalizationFile;
        
        // baseToggle = GUILayout.Toggle(baseToggle, "", EditorStyles.foldoutPreDrop);
        // if(baseToggle)
        //     base.OnInspectorGUI();

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
        obj.textStyle = EditorGUILayout.TextField("Fallback Text", obj.textStyle);

    }

    private void SetLocalizationCodeEnum(LocalizedText localizedText)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Localized Text Code",GUILayout.ExpandWidth(false));
        
        if (localizedText.editor_manualCode)
        {
            localizedText.localizationCode = GUILayout.TextArea(localizedText.localizationCode,GUILayout.ExpandWidth(true));
        }
        else if(localization!= null)
        {
            List<string> displayList = new List<string>();
            List<string> codeList = new List<string>();
            Dictionary<string, LanguageToString>.KeyCollection keys;

            switch (localizedText.textType)
            {
                case LocalizedTextTypes.UI:
                    keys = localization.uiDictionary.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {localization.GetUiText(key, "[not found]")}");
                        codeList.Add(key);
                    }
                        
                    
                    break;
                case LocalizedTextTypes.Story:
                    keys = localization.storyDictionary.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {localization.GetStoryText(key)}");
                        codeList.Add(key);
                    }
                        
                    
                    break;
                case LocalizedTextTypes.Achievement:
                    keys = localization.achievementDictionary.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {localization.GetAchievementName(key)}");
                        codeList.Add(key);
                    }
                        
                    
                    break;
                case LocalizedTextTypes.Music:
                    keys = localization.musicDictionary.Keys;

                    foreach (string key in keys)
                    {
                        displayList.Add($"{key} - {localization.GetMusicText(key)}");
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
