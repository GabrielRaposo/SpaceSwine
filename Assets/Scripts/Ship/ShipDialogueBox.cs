using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using RedBlueGames.Tools.TextTyper;
using DG.Tweening;
using TMPro;

public class ShipDialogueBox : MonoBehaviour
{
    const int BASE_WIDTH =  302;
    const int HIDDEN_Y   = -400;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event awardSoundAKEvent;

    [Header("References")]
    [SerializeField] TextTyper textTyper;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI textDisplaySimulator;
    [SerializeField] GameObject skipIcon;
    
    [Header("Images")]
    [SerializeField] Image closed2BGImage;
    [SerializeField] Image closed1BGImage;
    [SerializeField] Image openedBGImage;

    [Header("Rect Transforms")]
    [SerializeField] RectTransform largeBG;
    [SerializeField] RectTransform smallBG;
    [SerializeField] RectTransform verticalRivets;
    [SerializeField] RectTransform horizontalRivets;
    [SerializeField] RectTransform borders;
    [SerializeField] RectTransform textbox;
    [SerializeField] RectTransform textboxSimulator;

    public enum VerticalState { Closed, Open1, Open2 }
    VerticalState verticalState;

    bool shown;
    bool skippable;
    bool autoSkip;

    Sequence showSequence;
    Sequence verticalSequence;

    UnityAction afterInputAction;

    PlaySoundOnType playSoundOnType;
    PlayerInputActions inputActions;

    private void OnEnable() 
    {
        DisplaySkipIcon (false);

        playSoundOnType = GetComponentInChildren<PlaySoundOnType>();

        inputActions = new PlayerInputActions();

        inputActions.UI.Confirm.performed += SkipInput;
        inputActions.UI.Confirm.Enable();
    }

    void Start()
    {
        SetAllWidth(BASE_WIDTH);
        SetVerticalState(VerticalState.Closed);

        closed1BGImage.enabled = true; 
        openedBGImage.enabled = true;

        textDisplay.text = string.Empty;

        Hide();
    }


    private void OnDisable() 
    {
        inputActions.UI.Confirm.Disable();
    }

    #region IMMEDIATE

    private void Hide()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.up * HIDDEN_Y;
        shown = false;
    }

    private int BorderHeight (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return 60;
            case VerticalState.Open1:  return 80;
            case VerticalState.Open2:  return 98;
        }
    }

    private int RivetHeight (VerticalState state)
    {
        switch (state)
        {
            default:
            case VerticalState.Closed: return 84;
            case VerticalState.Open1:  return 104;
            case VerticalState.Open2:  return 122;
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
        verticalRivets.sizeDelta = verticalRivets.sizeDelta.SetX(width);
        horizontalRivets.sizeDelta = horizontalRivets.sizeDelta.SetX(width);
        borders.sizeDelta = borders.sizeDelta.SetX(width);
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

                verticalRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Closed));
                horizontalRivets.sizeDelta = new Vector2(borders.sizeDelta.x, RivetHeight(VerticalState.Closed));
                borders.sizeDelta = new Vector2(borders.sizeDelta.x, BorderHeight(VerticalState.Closed));

                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Closed)) );

                break;

            case VerticalState.Open1:
                
                closed1BGImage.color = new Color(1, 1, 1, 1);
                openedBGImage.color = new Color(1, 1, 1, 0);

                largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BGHeight(VerticalState.Open1));
                largeBG.transform.SetAsLastSibling();

                verticalRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Open1) );
                                horizontalRivets.sizeDelta = new Vector2(borders.sizeDelta.x, RivetHeight(VerticalState.Open1));
                borders.sizeDelta = new Vector2(borders.sizeDelta.x, BorderHeight(VerticalState.Open1));
        
                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Open1)) );
            
                break;

            case VerticalState.Open2:
                
                closed1BGImage.color = new Color(1, 1, 1, 1);
                openedBGImage.color = new Color(1, 1, 1, 0);

                largeBG.sizeDelta = smallBG.sizeDelta = largeBG.sizeDelta.SetY(BGHeight(VerticalState.Open2));
                largeBG.transform.SetAsLastSibling();

                verticalRivets.anchoredPosition = new Vector2(0, RivetY(VerticalState.Open2));
                horizontalRivets.sizeDelta = new Vector2(borders.sizeDelta.x, RivetHeight(VerticalState.Open2));
                borders.sizeDelta = new Vector2(borders.sizeDelta.x, BorderHeight(VerticalState.Open2));
        
                TextBoxSize( textbox.sizeDelta.SetY(TextHeight(VerticalState.Open2)) );
            
                break;
        }

        verticalState = vState;
    }

    #endregion

    #region TRANSITION

    public void SetShown (bool value, float duration = .5f, bool forceOut = false)
    {
        if (shown == value && !forceOut)
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
                    borders.sizeDelta = borders.sizeDelta
                        .SetY(ValueOnContext(f, BorderHeight(verticalState), BorderHeight(nextVerticalState)));
                    verticalRivets.anchoredPosition = verticalRivets.anchoredPosition
                        .SetY(ValueOnContext(f, RivetY(verticalState), RivetY(nextVerticalState)));
                    horizontalRivets.sizeDelta = horizontalRivets.sizeDelta
                        .SetY(ValueOnContext(f, RivetHeight(verticalState), RivetHeight(nextVerticalState)));
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

    public void Type 
    (
        string text, 
        float delay = .3f, 
        bool instantText = false,
        UnityAction afterInputAction = null
    )
    {
        this.afterInputAction = afterInputAction;

        if (textTyper.IsTyping)
            textTyper.Skip();

        textTyper.PrintCompleted.RemoveAllListeners();
        skippable = false;

        autoSkip = instantText;

        List<TextBoxTag> textTags = new List<TextBoxTag>();
        var parsedText = TextFunctions.ParseTags(text);
        if (parsedText.tags != null)
        {
            textTags = parsedText.tags;
            text = parsedText.output;
        }

        textDisplaySimulator.text = text;
        textDisplay.text = string.Empty;

        RaposUtil.Wait(this, frames: 2, () =>
        {
            int lineCount = textDisplaySimulator.textInfo.lineCount;

            if (lineCount < 3 && verticalState != VerticalState.Closed)
            {
                VerticalTransition(VerticalState.Closed, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, textTags));
                return;
            }

            if (lineCount == 3 && verticalState != VerticalState.Open1)
            {
                VerticalTransition(VerticalState.Open1, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, textTags));
                return;
            }

            if (lineCount >= 4 && verticalState != VerticalState.Open2)
            {
                VerticalTransition(VerticalState.Open2, delay);
                RaposUtil.WaitSeconds(this, delay, () => CallTyper(text, textTags));
                return;
            }
        
            CallTyper(text, textTags);
        });
    }

    private void DisplaySkipIcon(bool value)
    {
        if (!skipIcon)
            return;

        skipIcon.SetActive(value);
    }

    private void SkipInput(InputAction.CallbackContext ctx) 
    {
        if (!skippable)
            return;

        if (textTyper.IsTyping)
        {
            textTyper.Skip();
            return;
        }

        if (afterInputAction == null) 
            return;
        
        DisplaySkipIcon (false);
        afterInputAction();
    }

    private void CallTyper(string text, List<TextBoxTag> textTags)
    {
        if (playSoundOnType) 
            playSoundOnType.SetMute ( textTags.Contains(TextBoxTag.InstantText) );

        textTyper.TypeText(text);

        // -- Usa tags
        if (textTags.Contains(TextBoxTag.InstantText) || autoSkip)
            textTyper.Skip();
        
        if (textTags.Contains(TextBoxTag.AwardSound))   
        {
            if (awardSoundAKEvent != null)
                awardSoundAKEvent.Post(gameObject);
        }

        if (afterInputAction != null)
        {
            skippable = true;
            textTyper.PrintCompleted.AddListener( ()  => DisplaySkipIcon(true) ); // -- TO-DO: libera input de passar de texto 
        }
    }

    public void StopType()
    {
        if (textTyper.IsTyping)
            textTyper.Skip();

        textTyper.StopAllCoroutines();
    }

    #endregion
}
