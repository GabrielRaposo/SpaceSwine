﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum LocalizedTextTypes
{
    UI,
    Story,
    Achievement
}

public class LocalizedText : MonoBehaviour
{
    [SerializeField]private string localizationCode;
    [SerializeField] [TextArea(1,3)]private string fallbackText;

    public bool useUniversalFont;

    static string japaneseFontAdress = "Fonts/JP-Pomeranian-Regular SDF";
    static TMP_FontAsset japaneseFont;

    static string standardFontAdress = "Fonts/BaksoSapi SDF";
    static TMP_FontAsset standardFont;

    private string universalFontAdress = "Fonts/NotoSansMonoCJKjp_Universal";
    static TMP_FontAsset universalFont;

    public string textStyle;
    
    //public bool isStory;
    public LocalizedTextTypes textType;
    
    //[SerializeField] public TextMeshProUGUI textMesh;
    [SerializeField] public TMP_Text textMesh;


    private void OnEnable()
    {
        textMesh = GetComponent<TMP_Text>();
        
        if(textMesh == null) return;
        
        if (standardFont == null)
            standardFont = Resources.Load<TMP_FontAsset>(standardFontAdress);
        
        if (japaneseFont == null)
            japaneseFont = Resources.Load<TMP_FontAsset>(japaneseFontAdress);

        if (universalFont == null)
            universalFont = Resources.Load<TMP_FontAsset>(universalFontAdress);

        LocalizationManager.AddToList(this);
        
        SetText();
    }

    private void OnDisable()
    {
        LocalizationManager.RemoveFromList(this);
    }

    public void SetText()
    {
        if(textMesh == null) return;

        var file = LocalizationManager.LocalizationFile;

        if (file == null)
            Debug.Log("CANNOT FIND LOCALIZATION FILE");

        switch (textType)
        {
            case LocalizedTextTypes.UI:
                textMesh.text = file.GetUiText(localizationCode, fallbackText);
                break;
            case LocalizedTextTypes.Story:
                (bool valid, string text) story = file.GetStoryText(localizationCode);
                textMesh.text = story.text;
                break;
            case LocalizedTextTypes.Achievement:
                textMesh.text = file.GetAchievementText(localizationCode);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (!string.IsNullOrEmpty(textStyle))
            textMesh.text = textStyle.Replace("{s}", textMesh.text);

        if (useUniversalFont)
            textMesh.font = universalFont;
        else
        {
            if (LocalizationManager.CurrentLanguage == GameLocalizationCode.JP)
                textMesh.font = japaneseFont;
            else
                textMesh.font = standardFont;    
        }
    }

    public void SetText(string localizationCode)
    {
        this.localizationCode = localizationCode;
        SetText();
    }
}