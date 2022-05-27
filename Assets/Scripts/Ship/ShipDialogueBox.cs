using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;
using DG.Tweening;
using TMPro;

public class ShipDialogueBox : MonoBehaviour
{
    const int BASE_WIDTH =  302;
    const int HIDDEN_Y   = -400;

    [Header("References")]
    [SerializeField] TextTyper textTyper;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI textDisplaySimulator;
    
    [Header("Images")]
    [SerializeField] Image closed2BGImage;
    [SerializeField] Image closed1BGImage;
    [SerializeField] Image openedBGImage;

    [Header("Rect Transforms")]
    [SerializeField] RectTransform largeBG;
    [SerializeField] RectTransform smallBG;
    [SerializeField] RectTransform sideRivets;
    [SerializeField] RectTransform borderTop;
    [SerializeField] RectTransform borderBottom;
    [SerializeField] RectTransform textbox;
    [SerializeField] RectTransform textboxSimulator;

    public enum VerticalState { Closed, Open1, Open2 }
    VerticalState verticalState;

    bool shown;

    Sequence showSequence;
    Sequence verticalSequence;

    void Start()
    {
        SetAllWidth(BASE_WIDTH);
        SetVerticalState(VerticalState.Closed);

        closed1BGImage.enabled = true; 
        openedBGImage.enabled = true;

        textDisplay.text = string.Empty;

        Hide();
    }

    #region IMMEDIATE

    private void Hide()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.up * HIDDEN_Y;
        shown = false;
    }

    private int Height (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return 65;
            case VerticalState.Open1:  return 101;
            case VerticalState.Open2:  return 137;
        }
    }

    private int BGHeight (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return 44;
            case VerticalState.Open1:  return 62;
            case VerticalState.Open2:  return 80;
        }
    }

    private float RivetY (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return -8.5f;
            case VerticalState.Open1:  return  0.0f;
            case VerticalState.Open2:  return  8.5f;
        }
    }

    private int TextHeight (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return 85;
            case VerticalState.Open1:  return 101;
            case VerticalState.Open2:  return 119;
        }
    }

    private void TextBoxSize (Vector2 sizeDelta)
    {
        textbox.sizeDelta = textboxSimulator.sizeDelta = sizeDelta;
    }

    public void SetAllWidth (float width)
    {
        largeBG.sizeDelta = largeBG.sizeDelta.SetX(width);
        smallBG.sizeDelta = smallBG.sizeDelta.SetX(width);
        sideRivets.sizeDelta = sideRivets.sizeDelta.SetX(width);
        borderTop.sizeDelta = borderTop.sizeDelta.SetX(width);
        borderBottom.sizeDelta = borderBottom.sizeDelta.SetX(width);
        TextBoxSize(textbox.sizeDelta.SetX(width));
    }

    public void SetVerticalState(VerticalState vState)
    {
        switch (vState)
        {
            case VerticalState.Closed:
                
                closed1BGImage.color = new Color(1, 1, 1, 0);
                openedBGImage.color = new Color(1, 1, 1, 1);

                largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BGHeight(VerticalState.Closed));
                smallBG.transform.SetAsLastSibling();

                sideRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Closed));
                borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, Height(VerticalState.Closed));

                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Closed)) );

                break;

            case VerticalState.Open1:
                
                closed1BGImage.color = new Color(1, 1, 1, 1);
                openedBGImage.color = new Color(1, 1, 1, 0);

                largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BGHeight(VerticalState.Open1));
                largeBG.transform.SetAsLastSibling();

                sideRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Open1) );
                borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, Height(VerticalState.Open1));
        
                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Open1)) );
            
                break;

            case VerticalState.Open2:
                
                closed1BGImage.color = new Color(1, 1, 1, 1);
                openedBGImage.color = new Color(1, 1, 1, 0);

                largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BGHeight(VerticalState.Open2));
                largeBG.transform.SetAsLastSibling();

                sideRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Open2));
                borderTop.sizeDelta = new Vector2(borderTop.sizeDelta.x, Height(VerticalState.Open2));
        
                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Open2)) );
            
                break;
        }

        verticalState = vState;
    }

    #endregion

    #region TRANSITION

    public void SetShown (bool value, float duration = .5f)
    {
        if (shown == value)
            return;

        if (showSequence != null)
            showSequence.Kill();

        RectTransform r = GetComponent<RectTransform>();
        if (value)
            r.anchoredPosition = Vector2.up * HIDDEN_Y; 

        showSequence = DOTween.Sequence();
        showSequence.Append
        (
            r.DOAnchorPos
            (
                endValue: value ? Vector2.zero : new Vector2 (r.anchoredPosition.x, HIDDEN_Y),
                duration
            )
        );
        showSequence.OnComplete( () => shown = value );
    }

    private void SetVerticalSequence()
    {
        if (verticalSequence != null)
        {
            verticalSequence.Complete();
            verticalSequence.Kill();
        }

        verticalSequence = DOTween.Sequence();
    }

    public void VerticalTransition (VerticalState nextVerticalState, float duration)
    {
        SetVerticalSequence();

        SetVerticalState(verticalState);
        closed1BGImage.color = openedBGImage.color = new Color (1, 1, 1, 1);

        verticalSequence.Append
        (
            DOVirtual.Float
            (
                0.0f, 1.0f, duration, 
                (f) =>
                {
                    borderTop.sizeDelta = borderTop.sizeDelta
                        .SetY(ValueOnContext(f, Height(verticalState), Height(nextVerticalState)));
                    sideRivets.anchoredPosition = sideRivets.anchoredPosition
                        .SetY(ValueOnContext(f, RivetY(verticalState), RivetY(nextVerticalState)));
                    textbox.sizeDelta = textbox.sizeDelta
                        .SetY(ValueOnContext(f, TextHeight(verticalState), TextHeight(nextVerticalState)));
                    largeBG.sizeDelta = largeBG.sizeDelta
                        .SetY(ValueOnContext(f, BGHeight(verticalState), BGHeight(nextVerticalState)));
                    smallBG.sizeDelta = smallBG.sizeDelta
                        .SetY(ValueOnContext(f, BGHeight(verticalState), BGHeight(nextVerticalState)));
                }
            ).SetEase(Ease.Linear)
        );
        if (nextVerticalState != VerticalState.Closed)
            verticalSequence.Join ( openedBGImage.DOFade(0, duration) );
        else 
            verticalSequence.Join ( closed1BGImage.DOFade(0, duration) );

        verticalSequence.OnComplete( () => SetVerticalState(nextVerticalState) );
    }

    public void CloseTransition()
    {
        SetVerticalSequence();
    }

    private float ValueOnContext(float f, float start, float end)
    {
        return start + ((end - start) * Mathf.Clamp01(f));
    }

    #endregion

    #region TYPER

    public void Type (string text, float delay = .3f, bool instantText = false)
    {
        if (textTyper.IsTyping)
            textTyper.Skip();

        textDisplaySimulator.text = text;
        textDisplay.text = string.Empty;

        RaposUtil.Wait(this, frames: 2, () =>
        {
            int lineCount = textDisplaySimulator.textInfo.lineCount;

            if (lineCount < 3 && verticalState != VerticalState.Closed)
            {
                VerticalTransition(VerticalState.Closed, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, instantText));
                return;
            }

            if (lineCount == 3 && verticalState != VerticalState.Open1)
            {
                VerticalTransition(VerticalState.Open1, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, instantText));
                return;
            }

            if (lineCount >= 4 && verticalState != VerticalState.Open2)
            {
                VerticalTransition(VerticalState.Open2, delay);
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

    #endregion
}
