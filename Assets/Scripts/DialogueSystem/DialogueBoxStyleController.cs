using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBoxStyleController : MonoBehaviour
{
    [SerializeField] DialogueBoxStyle mainStyle;

    [Header("References")]
    [SerializeField] Image patternImage;
    [SerializeField] Image bgImage;
    [SerializeField] Image borderImage;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] GameObject nameBoxObject;

    //skip arrow anchor

    DialogueBoxStyle currentStyle;
    
    private void Start() 
    {
        SetMainStyle();
    }

    public void SetMainStyle()
    {
        SetStyle(mainStyle);
    }

    public void SetStyle (DialogueBoxStyle style)
    {
        if (style == currentStyle || style == null)
            return;

        switch(style.patternType)
        {
            case DialogueBoxStyle.PatternType.None:
                patternImage.enabled = false;
                break;

            case DialogueBoxStyle.PatternType.Squares:
                patternImage.enabled = true;
                break;
        }

        bgImage.color = style.BGColor;
        borderImage.sprite = style.borderSprite;
        borderImage.color = style.borderColor;

        textDisplay.color = style.textColor;
        textDisplay.fontSize = style.fontSize;
        textDisplay.enableAutoSizing = style.autoSize;
        textDisplay.font = style.fontAsset;
        
        // -- Margins
        float top = 30;
        if (style.customTopOffset > -1)
            top = style.customTopOffset;
        
        float left = 70;
        if (style.customLeftOffset > -1)
            left = style.customLeftOffset;

        textDisplay.margin = new Vector4( left, top, textDisplay.margin.z, textDisplay.margin.w);
        
        // -- Alignment
        textDisplay.horizontalAlignment = style.horizontalAlignment;
        textDisplay.verticalAlignment = style.verticalAlignment;

        nameBoxObject.SetActive(style.showNameBox);

        currentStyle = style;
    }
}
