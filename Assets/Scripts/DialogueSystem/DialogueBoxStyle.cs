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
    // border style
    // pattern style
    // skip arrow style
    // box anchor sizes

    public PatternType patternType;

    [Header("Colors")]
    public Color textColor = Color.white;
    public Color borderColor = Color.white;
    public Color BGColor = Color.white;

    [Header("Text")]
    public bool showNameBox = true;
    public float fontSize = 36f;
    public TMP_FontAsset fontAsset;
    public bool instantText = false;
}
