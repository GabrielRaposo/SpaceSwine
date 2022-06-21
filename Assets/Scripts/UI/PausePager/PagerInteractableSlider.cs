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
    [SerializeField] float maxRange;
    [SerializeField] UnityEvent<float> OnValueChanged;

    [Header("References")]
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] Image sliderBorder;
    [SerializeField] Image fillBar;
    [SerializeField] Image handler;

    float value;

    //private void Start() 
    //{
    //    // TO-DO: get valur from volume manager
    //    value = .5f;
       
    //    UpdateDisplay();
    //}

    public void InitiateValue (float value)
    {
        this.value = value;
        UpdateDisplay();
    }

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

    public override void OnHorizontalInput(float direction) 
    {
        float previousValue = value;

        value += .1f * direction;

        if (value < 0)
            value = 0;
        else if (value > 1)
            value = 1;

        if (value != previousValue)
            OnValueChanged?.Invoke(value);

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        float offset = 0f;
        
        if (fillBar)
        {
            fillBar.fillAmount = value - offset;
        }

        if (handler)
        {
            float anchoredX = (maxRange * 2.0f * value) - maxRange;
            handler.GetComponent<RectTransform>().anchoredPosition = Vector2.right * anchoredX; 
        }
    }
}
