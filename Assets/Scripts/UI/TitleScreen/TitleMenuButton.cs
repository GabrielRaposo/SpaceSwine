using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TitleMenuButton : InputSystemCompatibleButton
{
    [Header("Values")]
    [SerializeField] Color highlightColor;
    [SerializeField] Color hiddenColor;
    [SerializeField] float highlightX;
    [SerializeField] float hiddenX;
    [SerializeField] float duration;

    [Header("References")]
    [SerializeField] Image tabLine;
    [SerializeField] Image sideSlot;
    [SerializeField] CanvasGroup canvasGroup;

    
    Sequence s;
    private bool SlideSequence(bool value, UnityAction onComplete)
    {
        if (!tabLine)
            return false;

        if (s != null)
            s.Kill();
        
        s = DOTween.Sequence();
        s.Append ( tabLine.rectTransform.DOAnchorPosX(value ? highlightX : hiddenX, duration) );

        s.OnComplete(onComplete.Invoke);

        return true;
    }

    public override void Select (bool playSound = false)
    {
        if (playSound && hoverAKEvent != null)
            hoverAKEvent.Post(gameObject);

        bool value = true;
        if (! SlideSequence(value, () => SetState(selected = value) ))
            SetState (selected = value);
    }

    public override void Deselect() 
    {
        bool value = false;
        if (! SlideSequence(value, () => SetState(selected = value) ))
            SetState (selected = value);
    }

    protected override void SetState (bool value)
    {
        if (tabLine) 
        { 
            tabLine.color = value ? highlightColor : hiddenColor;
            tabLine.rectTransform.anchoredPosition = Vector2.right * (value ? highlightX : hiddenX);        
        }

        if (textDisplay)
            textDisplay.color = value ? highlightColor : hiddenColor;
    }

    public override void SetInteraction(bool value)
    {
        if (canvasGroup)
            canvasGroup.alpha = value ? 1.0f : .4f;
    }
}
