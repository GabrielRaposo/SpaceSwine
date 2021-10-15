using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;
using UnityEditor;
using UnityEngine.UI;

public enum GameLocalizationCode{EN, BR, JP, DE}; //Na mesma ordem da planilha

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
    
    private static string sheetId = "";
    private static string uiSheetName = "UI";
    private static string achievementSheetName = "Achievements"; 

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
        
        int storySheetsCount = 9;
        
        for (int i = 0; i < storySheetsCount; i++)
        {
            sheet = new GSTU_Search(sheetId, "Story"+i);
            SpreadsheetManager.Read(sheet, UpdateStoryDictionary);    
        }
        
        sheet = new GSTU_Search(sheetId, "SkillBooksStory");
        SpreadsheetManager.Read(sheet, UpdateStoryDictionary);

        sheet = new GSTU_Search(sheetId, uiSheetName);
        SpreadsheetManager.Read(sheet, UpdateUIDictionary);

        sheet = new GSTU_Search(sheetId, achievementSheetName);
        SpreadsheetManager.Read(sheet, UpdateAchievementDictionary);
        
    }
    
    private static void UpdateStoryDictionary(GstuSpreadSheet ss)
    {
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];
        
        Debug.Log($"lines count:{lines.Count}");
        
        for (int i = 2; i < lines.Count; i++)
        {
            var line = lines[i];

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
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile;
        file.uiDictionary = new CodeToDictionary();

        
        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i];

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
        var lines = ss.rows.primaryDictionary;
        
        var languageCodeList = Enum.GetValues(typeof(GameLocalizationCode)) as GameLocalizationCode[];

        var file = GetLocalizationFile;
        file.achievementDictionary = new CodeToDictionary();

        
        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i];

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
