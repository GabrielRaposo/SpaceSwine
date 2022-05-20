using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;
using DG.Tweening;
using TMPro;

public class ShipDialogueBox : MonoBehaviour
{
    #region CONST

    public const int BASE_WIDTH  = 302;
    
    public const int OPENED_HEIGHT = 101;
    public const int CLOSED_HEIGHT = 65;

    public const int BG_LARGE_HEIGHT = 62;
    public const int BG_SMALL_HEIGHT = 44;

    public const float RIVET_OPENED_Y = 0;
    public const float RIVET_CLOSED_Y = -8.5f;

    public const int CLOSED_TEXT_HEIGHT = 85;

    #endregion

    [Header("References")]
    [SerializeField] TextTyper textTyper;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI textDisplaySimulator;
    
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

    Sequence sequence;
    bool open;

    void Start()
    {
        SetAllWidth(BASE_WIDTH);
        SetClosedState();

        largeBGImage.enabled = true; 
        smallBGImage.enabled = true;

        textDisplay.text = string.Empty;
    }

    #region IMMEDIATE

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
        largeBGImage.color = new Color(1, 1, 1, 1);
        smallBGImage.color = new Color(1, 1, 1, 0);

        largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BG_LARGE_HEIGHT);
        largeBG.transform.SetAsLastSibling();

        sideRivets.anchoredPosition = new Vector2(0, RIVET_OPENED_Y);
        borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, OPENED_HEIGHT);
        
        textbox.sizeDelta = textbox.sizeDelta.SetY(OPENED_HEIGHT);

        open = true;
    }

    public void SetClosedState()
    {
        largeBGImage.color = new Color(1, 1, 1, 0);
        smallBGImage.color = new Color(1, 1, 1, 1);

        largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BG_SMALL_HEIGHT);
        smallBG.transform.SetAsLastSibling();

        sideRivets.anchoredPosition = new Vector2(0, RIVET_CLOSED_Y);
        borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, CLOSED_HEIGHT);

        textbox.sizeDelta = textbox.sizeDelta.SetY(CLOSED_TEXT_HEIGHT);

        open = false;
    }

    #endregion

    #region TRANSITION

    private void SetSequence()
    {
        if (sequence != null)
        {
            sequence.Complete();
            sequence.Kill();
        }

        sequence = DOTween.Sequence();
    }

    public void ToggleVerticalTransition(float duration = .5f)
    {
        if (sequence != null && sequence.IsPlaying())
            return;

        VerticalTransition(!open, duration);
    }

    public void VerticalTransition(bool opening, float duration = .5f)
    {
        SetSequence();

        if (opening) SetClosedState();
        else         SetOpenedState();

        largeBGImage.color = smallBGImage.color = new Color(1, 1, 1, 1);

        sequence.Append
        (
            DOVirtual.Float
            (
                0f, 1f, duration, 
                (f) =>
                {
                    float t = opening ? f : 1 - f;

                    borderTop.sizeDelta = borderTop.sizeDelta
                        .SetY(ValueOnContext(t, CLOSED_HEIGHT, OPENED_HEIGHT));
                    sideRivets.anchoredPosition = sideRivets.anchoredPosition
                        .SetY(ValueOnContext(t, RIVET_CLOSED_Y, RIVET_OPENED_Y));
                    textbox.sizeDelta = textbox.sizeDelta
                        .SetY(ValueOnContext(t, CLOSED_TEXT_HEIGHT, OPENED_HEIGHT));
                    largeBG.sizeDelta = largeBG.sizeDelta
                        .SetY(ValueOnContext(t, BG_SMALL_HEIGHT, BG_LARGE_HEIGHT));
                    smallBG.sizeDelta = smallBG.sizeDelta
                        .SetY(ValueOnContext(t, BG_SMALL_HEIGHT, BG_LARGE_HEIGHT));
                }
            )
        );
        if (opening)
            sequence.Join ( smallBGImage.DOFade(0, duration) );
        else 
            sequence.Join ( largeBGImage.DOFade(0, duration) );

        sequence.OnComplete( () => { if (opening) SetOpenedState(); else SetClosedState(); } );
    }

    public void CloseTransition()
    {
        SetSequence();
    }

    private float ValueOnContext(float f, float start, float end)
    {
        return start + ((end - start) * Mathf.Clamp01(f));
    }

    #endregion

    public void Type (string text, float delay = .5f, bool instantText = false)
    {
        if (textTyper.IsTyping)
            textTyper.Skip();

        textDisplaySimulator.text = text;
        textDisplay.text = string.Empty;

        RaposUtil.Wait(this, frames: 2, () =>
        {
            int lineCount = textDisplaySimulator.textInfo.lineCount;

            if (lineCount < 3 && open)
            {
                VerticalTransition(opening: false, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, instantText));
                return;
            }

            if (lineCount >= 3 && !open)
            {
                VerticalTransition(opening: true, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, instantText));
                return;
            }
        
            CallTyper(text);           
        });
    }

    private void CallTyper(string text, bool instantText = false)
    {
        textTyper.TypeText(text);

        if (instantText)
            textTyper.Skip();
    }
}
