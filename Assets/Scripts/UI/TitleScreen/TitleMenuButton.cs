using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class TitleMenuButton : MonoBehaviour
{
    [SerializeField] UnityEvent OnClickEvent;

    [Header("Values")]
    [SerializeField] Color highlightColor;
    [SerializeField] Color hiddenColor;
    [SerializeField] float highlightX;
    [SerializeField] float hiddenX;
    [SerializeField] float duration;

    [Header("References")]
    [SerializeField] Image tabLine;
    [SerializeField] Image sideSlot;
    [SerializeField] TextMeshProUGUI textDisplay;

    bool selected;
    
    Sequence s;

    private void Awake() 
    {
        SetState (selected = false);

        if (OnClickEvent == null)
            OnClickEvent = new UnityEvent();
    }

    public void Submit()
    {
        OnClickEvent.Invoke();
    }

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

    public void InstantSelect(bool value)
    {
        if (value)
            SetState( selected = true );
        else
            SetState( selected = false );
    }

    public void Select()
    {
        bool value = true;
        if (! SlideSequence(value, () => SetState(selected = value) ))
            SetState (selected = value);
    }

    public void Deselect() 
    {
        bool value = false;
        if (! SlideSequence(value, () => SetState(selected = value) ))
            SetState (selected = value);
    }

    private void SetState (bool value)
    {
        if (tabLine) 
        { 
            tabLine.color = value ? highlightColor : hiddenColor;
            tabLine.rectTransform.anchoredPosition = Vector2.right * (value ? highlightX : hiddenX);        
        }

        if (textDisplay)
            textDisplay.color = value ? highlightColor : hiddenColor;
    }
}
