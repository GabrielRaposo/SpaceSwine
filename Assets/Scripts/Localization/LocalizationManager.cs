using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;
using UnityEditor;
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

    private static GameLocalizationCode currentLanguage;

    private static List<LocalizedText> activeTexts;
    
    private static string sheetId = "1QHeZTAjHJxGhne9n63u-3B0Hmj0otwlVK8CM4czC2f8";
    private static string uiSheetName = "UI";
    private static string achievementSheetName = "Achievements";
    private static string musicSheetName = "Music";

    private static string localizationDataAddress = "Localization/LocalizationFile";
    
    static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static AstroPigLocalizationFile file;
    
    private static AstroPigLocalizationFile GetLocalizationFile => Resources.Load<AstroPigLocalizationFile>(localizationDataAddress);

    private static AstroPigLocalizationFile localizationFile;
    public static AstroPigLocalizationFile LocalizationFile
    {
        get
        {
            if (localizationFile != null) return localizationFile;

            localizationFile = GetLocalizationFile;

            if (localizationFile == null)
            {
                Debug.Log("LOCALIZATION FILE NOT FOUND ON MANAGER");
            }
            
            return localizationFile;
        }
    }
    public static void CallUpdate()
    {
        file = GetLocalizationFile;
        file.storyDictionary = new CodeToDictionary();

        GSTU_Search sheet;
        
        int zoneCount = 3;
        
        for (int i = 0; i < zoneCount; i++)
        {
            sheet = new GSTU_Search(sheetId, "Zona"+(i+1));
            SpreadsheetManager.Read(sheet, UpdateStoryDictionary);    
        }

        sheet = new GSTU_Search(sheetId, uiSheetName);
        SpreadsheetManager.Read(sheet, UpdateUIDictionary);

        sheet = new GSTU_Search(sheetId, achievementSheetName);
        SpreadsheetManager.Read(sheet, UpdateAchievementDictionary);

        sheet = new GSTU_Search(sheetId, musicSheetName);
        SpreadsheetManager.Read(sheet, UpdateMusicDictionary);

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
            
            file.storyDictionary.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
        
    }
    
    private static void UpdateUIDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateUIDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile;
        file.uiDictionary = new CodeToDictionary();

        
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
            
            file.uiDictionary.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
        
    }

    private static void UpdateAchievementDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateAchievementDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile;
        file.achievementDictionary = new CodeToDictionary();

        
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
            
            file.achievementDictionary.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
    }
    
    private static void UpdateMusicDictionary(GstuSpreadSheet ss)
    {
        Debug.Log("<color=#2277ff><b>UpdateAchievementDictionary()</b></color>");
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile;
        file.musicDictionary = new CodeToDictionary();

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
            
            file.musicDictionary.Add(code, languageToStringDictionary);
        }
        
        Debug.Log("Finished loading from GoogleSheets");
    }

    public static void AddToList(LocalizedText localizedText)
    {
        if(activeTexts == null)
            activeTexts = new List<LocalizedText>();
        
        activeTexts.Add(localizedText);
    }

    public static void RemoveFromList(LocalizedText localizedText)
    {
        if(activeTexts == null) return;

        activeTexts.Remove(localizedText);
    }
    
    private static void OnLanguageChange()
    {
        if(activeTexts == null) return;

        foreach (LocalizedText text in activeTexts)
            text.SetText();
    }

}
