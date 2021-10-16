using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AstroPigLocalizationFile", menuName = "Localization/AstroPigLocalizationFile")]
[System.Serializable]
public class AstroPigLocalizationFile : ScriptableObject
{
    public CodeToDictionary storyDictionary;
    public CodeToDictionary uiDictionary;
    public CodeToDictionary achievementDictionary;
    public CodeToDictionary musicDictionary;

    public (bool, string) GetStoryText(string code)
    {
        if (!storyDictionary.TryGetValue(code, out LanguageToString languageToString))
            return (false, $"[Story code not found: {code}]");

        if(!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
            return (false, "language not found");

        return (true, s);
    }

    public string GetUiText(string code, string fallback)
    {
        if (!uiDictionary.TryGetValue(code, out LanguageToString languageToString))
        {
            //Debug.Log($"UI fallback {code}");
            return fallback;
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            Debug.Log($"UI fallback {code}");
            return fallback;
        }

        return s;
    }

    public string GetAchievementName(string id)
    {
        if (!achievementDictionary.TryGetValue(id+"_TITLE", out LanguageToString languageToString))
        {
            return $"Achievement name not found {id}";
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            return $"Achievement name not found {id}";
        }
        return s;
    }
    
    public string GetAchievementDescription(string id)
    {
        if (!achievementDictionary.TryGetValue(id+"_DESC", out LanguageToString languageToString))
        {
            return $"Achievement description not found {id}";
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            return $"Achievement description not found {id}";
        }
        return s;
    }

    public string GetMusicText(string localizationCode)
    {
        return "";
        //throw new System.NotImplementedException();
    }
}
