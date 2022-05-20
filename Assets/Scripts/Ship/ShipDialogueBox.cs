using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;
using DG.Tweening;
using TMPro;

public class ShipDialogueBox : MonoBehaviour
{
    [SerializeField] TextTyper textTyper;
    [SerializeField] TextMeshProUGUI textDisplay;
    
    [Header("Images")]
    [SerializeField] Image largeBGImage;
    [SerializeField] Image smallBGImage;

    [Header("Rect Transforms")]
    [SerializeField] RectTransform largeBG;
    [SerializeField] RectTransform smallBG;
    [SerializeField] RectTransform sideRivets;
    [SerializeField] RectTransform borderTop;
    [SerializeField] RectTransform borderBottom;
    [SerializeField] RectTransform textbox;

    public const int BASE_WIDTH  = 302;
    
    public const int OPENED_HEIGHT = 101;
    public const int CLOSED_HEIGHT = 65;

    public const int RIVET_OPENED_Y = 0;
    public const int RIVET_CLOSED_Y = -17;

    public const int CLOSED_TEXT_HEIGHT = 85;

    void Start()
    {
        SetAllWidth(BASE_WIDTH);
        SetClosedState();
    }

    public void SetAllWidth (float width)
    {
        largeBG.sizeDelta = largeBG.sizeDelta.SetX(width);
        smallBG.sizeDelta = smallBG.sizeDelta.SetX(width);
        sideRivets.sizeDelta = sideRivets.sizeDelta.SetX(width);
        borderTop.sizeDelta = borderTop.sizeDelta.SetX(width);
        borderBottom.sizeDelta = borderBottom.sizeDelta.SetX(width);
        textbox.sizeDelta = textbox.sizeDelta.SetX(width);
    }

    public void SetOpenedState()
    {
        largeBGImage.enabled = true; 
        smallBGImage.enabled = false;

        sideRivets.anchoredPosition = new Vector2(0, RIVET_OPENED_Y);
        borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, OPENED_HEIGHT);
        
        textbox.sizeDelta = textbox.sizeDelta.SetY(OPENED_HEIGHT);
    }

    public void SetClosedState()
    {
        largeBGImage.enabled = false; 
        smallBGImage.enabled = true;

        sideRivets.anchoredPosition = new Vector2(0, RIVET_CLOSED_Y);
        borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, CLOSED_HEIGHT);

        textbox.sizeDelta = textbox.sizeDelta.SetY(CLOSED_TEXT_HEIGHT);
    }

}
