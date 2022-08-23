using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class PagerInteractableScrollList : PagerInteractable
{
    [Header("Values")]
    [SerializeField] Color lightColor;
    [SerializeField] Color darkColor;
    [SerializeField] UnityEvent<int> OnValueChanged;

    [Header("References")]
    [SerializeField] TextMeshProUGUI labelDisplay;
    [SerializeField] Image background;
    [SerializeField] Image border;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;

    int current;

    List <string> list = new List<string>();
    
    public void InitList(List<string> list, int current)
    {
        this.list = list;
        this.current = current;

        UpdateDisplay();
    }

    public override void Select() 
    {
        if (background)
            background.color = darkColor;

        if (labelDisplay)
            labelDisplay.color = lightColor;

        if (textDisplay)
            textDisplay.color = lightColor;

        if (border)
            border.color = lightColor;

        if (leftArrow)
            leftArrow.color = lightColor;

        if (rightArrow)
            rightArrow.color = lightColor;
    }

    public override void Deselect() 
    {
        if (background)
            background.color = Color.clear;

        if (labelDisplay)
            labelDisplay.color = darkColor;

        if (textDisplay)
            textDisplay.color = darkColor;

        if (border)
            border.color = darkColor;

        if (leftArrow)
            leftArrow.color = darkColor;

        if (rightArrow)
            rightArrow.color = darkColor;
    }

    public void UpdateDisplay() 
    {
        if (list.Count < 1)
            return;

        if (textDisplay == null)
            return;

        string text = list[current % list.Count];
        textDisplay.text = text;

        if (OnValueChanged != null)
            OnValueChanged.Invoke( current % list.Count );
    }

    public override bool OnHorizontalInput(float direction) 
    {
        if (direction > 0)
        {
            MoveIndex(1);
            return true;
        }

        if (direction < 0)
        {
            MoveIndex(-1);
            return true;
        }
        
        return false;
    }

    public void MoveIndex (int direction)
    {
        if (list.Count < 1)
            return;

        current += direction;

        if (current < 0)
            current = list.Count - 1;
        current %= list.Count;

        if (direction < 0 && leftArrow)
        {
            RectTransform leftRT = leftArrow.GetComponent<RectTransform>();
            leftRT.DOKill();
            leftRT.anchoredPosition = Vector2.left * 6;
            leftRT.DOPunchAnchorPos(Vector2.left * 2, .1f, vibrato: 0).SetUpdate(isIndependentUpdate: true);
        }

        if (direction > 0 && rightArrow)
        {
            RectTransform rightRT = rightArrow.GetComponent<RectTransform>();
            rightRT.DOKill();
            rightRT.anchoredPosition = Vector2.right * 6;
            rightRT.DOPunchAnchorPos(Vector2.right * 2, .1f, vibrato: 0).SetUpdate(isIndependentUpdate: true);
        }

        UpdateDisplay();
    }
}
