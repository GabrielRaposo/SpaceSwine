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
    [SerializeField] Image skipIconImage;
    [SerializeField] RectTransform skipIconAnchor;
    [SerializeField] TextMeshProUGUI textDisplay;
    
    [Header("Name Box")]
    [SerializeField] GameObject nameBoxObject;
    [SerializeField] Image nameBoxImage;

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

        if (style.skipIconSprite != null)
            skipIconImage.sprite = style.skipIconSprite;
        skipIconImage.color = style.skipIconColor;
        
        RectTransform iconAnchorRT = skipIconAnchor.GetComponent<RectTransform>();
        if (iconAnchorRT)
        {
            float X = 510;
            if (style.skipIconPosX != -1)
                X = style.skipIconPosX;

            float Y = -54;
            if (style.skipIconPosY != -1)
                Y = style.skipIconPosY;

            iconAnchorRT.anchoredPosition = new Vector2 (X, Y);
        }
        Image iconImage = skipIconAnchor.GetComponentInChildren<Image> ();
        if (iconImage)
        {
            float W = 46.875f;
            if (style.skipIconWidth != -1)
                W = style.skipIconWidth;

            float H = 40.625f;
            if (style.skipIconHeight != -1)
                H = style.skipIconHeight;

            iconImage.GetComponent<RectTransform>().sizeDelta = new Vector2 (W, H);
        }

        textDisplay.color = style.textColor;
        textDisplay.fontSize = style.fontSize;
        textDisplay.enableAutoSizing = style.autoSize;
        textDisplay.font = style.fontAsset;
        
        // -- Text Area

        // -- Margins
        float top = 30;
        if (style.customTopOffset > -1)
            top = style.customTopOffset;
        
        float left = 70;
        if (style.customLeftOffset > -1)
            left = style.customLeftOffset;

        float botom = 40;
        if (style.customBottomOffset > -1)
            botom = style.customBottomOffset;

        float right = 70;
        if (style.customRightOffset > -1)
            right = style.customRightOffset;

        textDisplay.margin = new Vector4( left, top, right, botom );
        
        // -- Alignment
        textDisplay.horizontalAlignment = style.horizontalAlignment;
        textDisplay.verticalAlignment = style.verticalAlignment;
        
        // -- Name Box
        nameBoxObject.SetActive(style.showNameBox);
        nameBoxImage.sprite = style.nameBoxSprite;
        nameBoxImage.enabled = style.nameBoxSprite;
        nameBoxObject.GetComponent<RectTransform>().localPosition = new Vector2 (style.nameBoxPosX, style.nameBoxPosY);

        currentStyle = style;
    }
}
