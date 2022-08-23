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
    // skip arrow style
    // box anchor sizes

    public PatternType patternType;
    public Sprite borderSprite;
    public Sprite skipIconSprite;

    [Header("Colors")]
    public Color textColor = Color.white;
    public Color borderColor = Color.white;
    public Color BGColor = Color.white;
    public Color skipIconColor = Color.white;

    [Header("Text")]
    public bool showNameBox = true;
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
