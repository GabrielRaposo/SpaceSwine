using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum LocalizedTextTypes
{
    UI,
    Story,
    Achievement,
    Music,
    Nave
}

public class LocalizedText : MonoBehaviour
{
    public string localizationCode;
    [TextArea(1,3)]public string fallbackText;

    //public bool useUniversalFont;

    // static string japaneseFontAdress = "Fonts/JP-Pomeranian-Regular SDF";
    // static TMP_FontAsset japaneseFont;

    // static string standardFontAdress = "Fonts/BaksoSapi SDF";
    // static TMP_FontAsset standardFont;

    // private string universalFontAdress = "Fonts/NotoSansMonoCJKjp_Universal";
    // static TMP_FontAsset universalFont;

    public string textStyle;
    
    //public bool isStory;
    public LocalizedTextTypes textType;
    
    //[SerializeField] public TextMeshProUGUI textMesh;
    [SerializeField] public TMP_Text textMesh;

    
    //EDITOR
    public bool editor_manualCode;

    private void OnEnable()
    {
        textMesh = GetComponent<TMP_Text>();
        
        if(textMesh == null) return;
        
        // if (standardFont == null)
        //     standardFont = Resources.Load<TMP_FontAsset>(standardFontAdress);
        
        // if (japaneseFont == null)
        //     japaneseFont = Resources.Load<TMP_FontAsset>(japaneseFontAdress);
        //
        // if (universalFont == null)
        //     universalFont = Resources.Load<TMP_FontAsset>(universalFontAdress);

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

        switch (textType)
        {
            case LocalizedTextTypes.UI:
                textMesh.text = LocalizationManager.GetUiText(localizationCode, fallbackText);
                break;
            case LocalizedTextTypes.Story:
                (bool valid, string text) story = LocalizationManager.GetStoryText(localizationCode);
                textMesh.text = story.text;
                break;
            case LocalizedTextTypes.Achievement:
                textMesh.text = LocalizationManager.GetAchievementName(localizationCode);
                break;
            case LocalizedTextTypes.Music:
                textMesh.text = LocalizationManager.GetMusicText(localizationCode);
                break;
            case LocalizedTextTypes.Nave:
                (bool valid, string text) ship = LocalizationManager.GetShipText(localizationCode);
                textMesh.text = ship.text;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (!string.IsNullOrEmpty(textStyle))
            textMesh.text = textStyle.Replace("{s}", textMesh.text);

        // if (useUniversalFont)
        //     textMesh.font = universalFont;
        else
        {
            // if (LocalizationManager.CurrentLanguage == GameLocalizationCode.JP)
            //     textMesh.font = japaneseFont;
            // else
            //    textMesh.font = standardFont;    
        }
    }

    public void SetText(string localizationCode)
    {
        this.localizationCode = localizationCode;
        SetText();
    }
}
