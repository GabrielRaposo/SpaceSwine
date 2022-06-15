using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PagerInteractableSlider : PagerInteractable
{
    [Header("Values")]
    [SerializeField] Color lightColor;
    [SerializeField] Color darkColor;
    [SerializeField] UnityEvent interactionEvent;

    [Header("References")]
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] Image sliderBorder;
    [SerializeField] Image fillBar;
    [SerializeField] Image handler;

    public override void Select() 
    {
        if (background)
            background.color = darkColor;

        if (textDisplay)
            textDisplay.color = lightColor;

        if (sliderBorder)
            sliderBorder.color = lightColor;

        if (fillBar)
            fillBar.color = lightColor;

        if (handler)
            handler.color = lightColor;
    }

    public override void Deselect() 
    {
        if (background)
            background.color = Color.clear;

        if (textDisplay)
            textDisplay.color = darkColor;

        if (sliderBorder)
            sliderBorder.color = darkColor;

        if (fillBar)
            fillBar.color = darkColor;

        if (handler)
            handler.color = darkColor;
    }
}
