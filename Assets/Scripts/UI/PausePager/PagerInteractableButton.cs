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
    [SerializeField] Image extraAsset;

    public override void Select() 
    {
        if (border)
            border.color = lightColor;

        if (background)
            background.color = darkColor;

        if (textDisplay)
            textDisplay.color = lightColor;

        if (extraAsset)
            extraAsset.color = lightColor;
    }

    public override void Deselect() 
    {
        if (border)
            border.color = darkColor;

        if (background)
            background.color = Color.clear;

        if (textDisplay)
            textDisplay.color = darkColor;

        if (extraAsset)
            extraAsset.color = darkColor;
    }

    public override void OnClick() 
    {
        if (interactionEvent != null)
            interactionEvent.Invoke();
    }

    public void OverrideInteractionEvent(UnityAction call)
    {
        if (interactionEvent != null)
        {
            interactionEvent.RemoveAllListeners();
            interactionEvent.AddListener(call);
        }
    }
}
