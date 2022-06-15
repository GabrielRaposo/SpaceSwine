using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PagerInteractableButton : PagerInteractable
{
    [Header("Values")]
    [SerializeField] Color lightColor;
    [SerializeField] Color darkColor;
    [SerializeField] UnityEvent interactionEvent;

    [Header("References")]
    [SerializeField] Image border;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI textDisplay;

    public override void Select() 
    {
        if (border)
            border.color = lightColor;

        if (background)
            background.color = darkColor;

        if (textDisplay)
            textDisplay.color = lightColor;
    }

    public override void Deselect() 
    {
        if (border)
            border.color = darkColor;

        if (background)
            background.color = Color.clear;

        if (textDisplay)
            textDisplay.color = darkColor;
    }

    public override void OnClick() 
    {
        if (interactionEvent != null)
            interactionEvent.Invoke();
    }
}
