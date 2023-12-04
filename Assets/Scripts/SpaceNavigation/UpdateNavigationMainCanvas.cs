﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateNavigationMainCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI autoPilotText;
    [SerializeField] Image bannerBackgroundImage;
    [SerializeField] Image upperBarImage;
    [SerializeField] TextMeshProUGUI tabTextDisplay;
    [SerializeField] Image lowerBarImage;

    public void UpdateColors (Color selectedColor, Color unselectedColor, Color backgroundColor)
    {
        Color aux = Vector4.zero;

        //if (autoPilotText != null) 
        //    autoPilotText.color = selectedColor;

        if (bannerBackgroundImage != null) 
        {
            aux = bannerBackgroundImage.color;
            aux.a = bannerBackgroundImage.color.a;

            bannerBackgroundImage.color = aux;
        }

        if (upperBarImage != null) 
        {
            upperBarImage.color = selectedColor;
        }

        if (tabTextDisplay != null) 
        {
            tabTextDisplay.color = selectedColor;
        }

        if (lowerBarImage != null) 
        {
            lowerBarImage.color = selectedColor;
        }
    }
}