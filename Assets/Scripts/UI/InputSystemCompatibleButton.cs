using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public abstract class InputSystemCompatibleButton : MonoBehaviour
{
    public UnityEvent OnClickEvent;
    
    [Header("Audio")]
    [SerializeField] protected AK.Wwise.Event hoverAKEvent;
    [SerializeField] protected AK.Wwise.Event clickAKEvent;
    
    [SerializeField] public TextMeshProUGUI textDisplay;
    
    protected bool selected;

    private void Awake() 
    {
        SetState (selected = false);

        if (OnClickEvent == null)
            OnClickEvent = new UnityEvent();
    }
    
    public void Submit()
    {
        //Debug.Log("Submit: " + name);
        
        if (clickAKEvent != null)
            clickAKEvent.Post(gameObject);

        OnClickEvent.Invoke();
    }
    public void SetEvent(Action onClick)
    {
        OnClickEvent = new UnityEvent();
        OnClickEvent.AddListener(()=>onClick());
    }

    public void RemoveAllListeners()
    {
        OnClickEvent.RemoveAllListeners();
    }
    
    protected abstract void SetState(bool value);
    public abstract void SetInteraction(bool value);
    
    
    
    public void InstantSelect (bool value)
    {
        if (value)
            SetState( selected = true );
        else
            SetState( selected = false );
    }

    public abstract void Select(bool playSound = false);
    public abstract void Deselect();
}
