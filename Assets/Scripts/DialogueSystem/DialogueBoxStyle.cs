using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu (fileName = "DialogBoxStyle_", menuName = "ScriptableObjects/DialogBoxStyle") ]
public class DialogueBoxStyle : ScriptableObject
{
    public enum PatternType
    {
        None, Squares
    }

    // typing speed
    // pattern style
    // box anchor sizes

    public PatternType patternType;
    public Sprite borderSprite;

    [Header("Colors")]
    public Color textColor = Color.white;
    public Color borderColor = Color.white;
    public Color BGColor = Color.white;
    public Color skipIconColor = Color.white;

    [Header("Skip Icon")]
    public Sprite skipIconSprite;
    public float skipIconPosX = -1;
    public float skipIconPosY = -1;

    [Header("Name Box")]
    public bool showNameBox = true;
    public Sprite nameBoxSprite;
    public float nameBoxPosX = -394f;
    public float nameBoxPosY = 104f;

    [Header("Font")]
    public float fontSize = 36f;
    public bool autoSize = false;
    public TMP_FontAsset fontAsset;
    public bool instantText = false;

    [Header("Text Alignment")]
    public float customLeftOffset = -1;
    public float customTopOffset = -1;
    public float customRightOffset = -1;
    public float customBottomOffset = -1;
    public HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center;
    public VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Top;
}
