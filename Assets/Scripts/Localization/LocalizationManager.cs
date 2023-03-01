using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GoogleSheetsToUnity;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GameLocalizationCode{EN, BR}; //Na mesma ordem da planilha

public static class LocalizationManager
{
    public static bool[] EnabledLanguages =
    {
        true,   //EN
        true,   //BR
        false,  //JP
        false   //DE
    };
    
    public static GameLocalizationCode CurrentLanguage
    {
        get => currentLanguage;
        set
        {
            Debug.Log("Change language to " + value);
            if(value == currentLanguage) return;
            currentLanguage = value;

            if (!EnabledLanguages[(int) currentLanguage])
            {
                if ((int) currentLanguage == Enum.GetNames(typeof(GameLocalizationCode)).Length - 1)
                    CurrentLanguage = 0;
                else
                    CurrentLanguage = (GameLocalizationCode)(((int) currentLanguage) + 1);
            }
            
            OnLanguageChange();
        }
    }

    public static AstroPigLocalizationFile StoryFile => GetLocalizationFile(LocalizedTextTypes.Story);

    public static AstroPigLocalizationFile UiFile => GetLocalizationFile(LocalizedTextTypes.UI);

    public static AstroPigLocalizationFile InputFile => GetLocalizationFile(LocalizedTextTypes.Inputs);
    public static AstroPigLocalizationFile MusicFile => GetLocalizationFile(LocalizedTextTypes.Music);

    public static AstroPigLocalizationFile AchievementsFile => GetLocalizationFile(LocalizedTextTypes.Achievement);

    public static AstroPigLocalizationFile NaveFile => GetLocalizationFile(LocalizedTextTypes.Nave);

    
    private static GameLocalizationCode currentLanguage;

    private static List<LocalizedText> activeTexts;
    private static List<UnityAction> onChangeLanguage;

    private static string sheetId = "1QHeZTAjHJxGhne9n63u-3B0Hmj0otwlVK8CM4czC2f8";
    
    private static string uiSheetName = "UI";
    private static string inputSheetName = "Input";
    private static string achievementSheetName = "Achievements";
    private static string musicSheetName = "Music";
    private static string naveSheetName = "Nave";

    static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static AstroPigLocalizationFile storyFile;
    private static AstroPigLocalizationFile uiFile;
    private static AstroPigLocalizationFile inputFile;
    private static AstroPigLocalizationFile musicFile;
    private static AstroPigLocalizationFile achievementsFile;
    private static AstroPigLocalizationFile naveFile;
    
    
    private static AstroPigLocalizationFile GetLocalizationFile(LocalizedTextTypes textType)
    {
        string localizationDataAddress = "Localization/LocalizationFile";

        switch (textType)
        {
            case LocalizedTextTypes.UI:
                if (uiFile != null) return uiFile;
                localizationDataAddress += "_UI";
                break;
            
            case LocalizedTextTypes.Inputs:
                if (inputFile != null) return inputFile;
                localizationDataAddress += "_Input";
                break;
            
            case LocalizedTextTypes.Story:
                if (storyFile != null) return storyFile;
                localizationDataAddress += "_Story";
                break;
            case LocalizedTextTypes.Achievement:
                if (achievementsFile != null) return achievementsFile;
                localizationDataAddress += "_Achievement";
                break;
            case LocalizedTextTypes.Music:
                if (musicFile != null) return musicFile;
                localizationDataAddress += "_Music";
                break;
            case LocalizedTextTypes.Nave:
                if (naveFile != null) return naveFile;
                localizationDataAddress += "_Nave";
                break;
        }
        
        var file = Resources.Load<AstroPigLocalizationFile>(localizationDataAddress);

        if (file == null)
        {
            Debug.Log($"File not found:{localizationDataAddress}");
            return null;
        }
        
        switch (textType)
        {
            case LocalizedTextTypes.UI:
                uiFile = file;
                return uiFile;
                
            case LocalizedTextTypes.Inputs:
                inputFile = file;
                return inputFile;
            
            case LocalizedTextTypes.Story:
                storyFile = file;
                return storyFile;
            
            case LocalizedTextTypes.Achievement:
                achievementsFile = file;
                return achievementsFile;
            
            case LocalizedTextTypes.Music:
                musicFile = file;
                return musicFile;
            
            case LocalizedTextTypes.Nave:
                naveFile = file;
                return naveFile;
        }

        return null;

    }

    public static AstroPigLocalizationFile LocalizationFile(LocalizedTextTypes textType)
    {
        var file = GetLocalizationFile(textType);

        return file;
    }
    
    public static async Task CallUpdate(LocalizedTextTypes textType, Action OnComplete)
    {
        GSTU_Search sheet;
        
        int zoneCount = 3;

        var completionSource = new TaskCompletionSource<int>();
        
        switch (textType)
        {
            case LocalizedTextTypes.Story:
                
                storyFile = GetLocalizationFile(LocalizedTextTypes.Story);
                storyFile.dic = new CodeToDictionary();

                for (int i = -1; i < zoneCount; i++)
                {
                    var storyPageCompletionSource = new TaskCompletionSource<int>();
                    Debug.Log("Zona"+(i+1));
                    sheet = new GSTU_Search(sheetId, "Zona"+(i+1));
                    SpreadsheetManager.Read(sheet, x=>{UpdateStoryDictionary(x);storyPageCompletionSource.SetResult(123);});
                    await storyPageCompletionSource.Task;
                }
                break;
            
            case LocalizedTextTypes.UI:
                
                sheet = new GSTU_Search(sheetId, uiSheetName);
                SpreadsheetManager.Read(sheet, x=>{UpdateUIDictionary(x);completionSource.SetResult(123);});
                await completionSource.Task;
                break;
            
            case LocalizedTextTypes.Inputs:

                sheet = new GSTU_Search(sheetId, inputSheetName);
                SpreadsheetManager.Read(sheet, x=>{UpdateInputDictionary(x); completionSource.SetResult(123);});
                await completionSource.Task;
                break;
            
            case LocalizedTextTypes.Achievement:
                
                sheet = new GSTU_Search(sheetId, achievementSheetName);
                SpreadsheetManager.Read(sheet, x=>{UpdateAchievementDictionary(x);completionSource.SetResult(123);});
                await completionSource.Task;
                break;
            
            case LocalizedTextTypes.Music:
                
                sheet = new GSTU_Search(sheetId, musicSheetName);
                SpreadsheetManager.Read(sheet, x=>{UpdateMusicDictionary(x);completionSource.SetResult(123);});
                await completionSource.Task;
                break;
            
            case LocalizedTextTypes.Nave:

                sheet = new GSTU_Search(sheetId, naveSheetName);
                SpreadsheetManager.Read(sheet, x=>{UpdateNaveDictionary(x);completionSource.SetResult(123);});
                await completionSource.Task;
                break;
        }
        
        OnComplete.Invoke();
    }
    
    
    private static void UpdateStoryDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateStoryDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            storyFile.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
        
    }
    
    private static void UpdateUIDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateUIDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        uiFile = GetLocalizationFile(LocalizedTextTypes.UI);
        uiFile.dic = new CodeToDictionary();

        
        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            uiFile.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
        
    }
    
    private static void UpdateInputDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateInputDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        inputFile = GetLocalizationFile(LocalizedTextTypes.Inputs);
        inputFile.dic = new CodeToDictionary();

        
        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            inputFile.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
        
    }

    private static void UpdateAchievementDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateAchievementDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile(LocalizedTextTypes.Achievement);
        file.dic = new CodeToDictionary();

        
        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            file.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
    }
    
    private static void UpdateMusicDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateAchievementDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile(LocalizedTextTypes.Music);
        file.dic = new CodeToDictionary();

        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            file.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
    }
    
    private static void UpdateNaveDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateNaveDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile(LocalizedTextTypes.Nave);
        file.dic = new CodeToDictionary();

        for (int i = 2; i < lines.Count+1; i++)
        {
            var line = lines[i];
            Debug.Log($"<b>Line {i}</b>: {line[0].value}");

            string code = line[0].value;

            LanguageToString languageToStringDictionary = new LanguageToString();

            for (int j = 0; j < languageCodeList.Length; j++)
            {
                int columnNumber = j + 1;

                GameLocalizationCode glc = languageCodeList[j];
                languageToStringDictionary.Add(glc, line[columnNumber].value);
            }
            
            file.dic.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
    }

    public static void AddToActiveTextList(LocalizedText localizedText)
    {
        if(activeTexts == null)
            activeTexts = new List<LocalizedText>();
        
        activeTexts.Add(localizedText);
    }

    public static void RemoveFromActiveTextList(LocalizedText localizedText)
    {
        if(activeTexts == null) return;

        activeTexts.Remove(localizedText);
    }

    public static void AddToLanguageChangeActionList(UnityAction a)
    {
        if (onChangeLanguage == null)
            onChangeLanguage = new List<UnityAction>();
        
        onChangeLanguage.Add(a);
    }

    public static void RemoveFromLanguageChangeActionList(UnityAction a)
    {
        if(onChangeLanguage == null)
            return;

        onChangeLanguage.Remove(a);
    }
    
    private static void OnLanguageChange()
    {
        if (activeTexts != null)
        {
            foreach (LocalizedText text in activeTexts)
                text.SetText();    
        }

        if (onChangeLanguage != null)
        {
            foreach (UnityAction action in onChangeLanguage)
                action?.Invoke();
        }

        
    }
    
    /////////
    
    public static (bool, string) GetStoryText(string code)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.Story).dic.TryGetValue(code, out LanguageToString languageToString))
            return (false, $"[Story code not found: {code}]");

        if(!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
            return (false, "language not found");

        return (true, s);
    }

    public static string GetUiText(string code, string fallback)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.UI).dic.TryGetValue(code, out LanguageToString languageToString))
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

    public static string GetInputText(string code)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.Inputs).dic.TryGetValue(code, out LanguageToString languageToString))
        {
            return code;
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            Debug.Log($"Input fallback {code}");
            return code;
        }

        return s;
    }

    public static string GetAchievementName(string id)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.Achievement).dic.TryGetValue(id+"_TITLE", out LanguageToString languageToString))
        {
            return $"Achievement name not found {id}";
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            return $"Achievement name not found {id}";
        }
        return s;
    }
    
    public static string GetAchievementDescription(string id)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.Achievement).dic.TryGetValue(id+"_DESC", out LanguageToString languageToString))
        {
            return $"Achievement description not found {id}";
        }

        if (!languageToString.TryGetValue(LocalizationManager.CurrentLanguage, out string s))
        {
            return $"Achievement description not found {id}";
        }
        return s;
    }

    public static string GetMusicText(string localizationCode)
    {
        //GetLocalizationFile(LocalizedTextTypes.Music)
        return "";
        //throw new System.NotImplementedException();
    }

    public static (bool, string) GetShipText(string localizationCode)
    {
        if (!GetLocalizationFile(LocalizedTextTypes.Nave).dic
                .TryGetValue(localizationCode, out LanguageToString languageToString))
        {
            return (false, $"Nave text not found {localizationCode}");
        }

        if (!languageToString.TryGetValue(CurrentLanguage, out string s))
        {
            return (false, $"Nave text not found {localizationCode}");
        }

        return (true, s);
    }
}
