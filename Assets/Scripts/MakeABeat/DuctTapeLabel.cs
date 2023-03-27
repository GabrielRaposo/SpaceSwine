using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DuctTapeLabel : MonoBehaviour
{
    [SerializeField] Vector2 baseSize;

    Color textColor;

    SpriteRenderer spriteDisplay;
    TextMeshPro textDisplay;

    private void Awake() 
    {
        spriteDisplay = GetComponentInChildren<SpriteRenderer>();
        textDisplay = GetComponentInChildren<TextMeshPro>();

        if (!spriteDisplay || !textDisplay)
        {
            gameObject.SetActive(false);
            return;
        }

        textColor = textDisplay.color;

        Hide();
    }

    public void SetVisibility (bool value)
    {
        value = value && textDisplay.text != string.Empty;
        
        spriteDisplay.enabled = value;
        textDisplay.color = value ? textColor : Color.clear;
    }

    public void SetText (string text)
    {
        textDisplay.text = text;
        RaposUtil.Wait(this, 1, UpdateSpriteSize);
    }

    public void Show ()
    {
        StopAllCoroutines();
        //SetVisibility(false);

        RaposUtil.Wait(this, 1, 
            () => { 
                UpdateSpriteSize(); 
                SetVisibility(true);
            } 
        );
    }

    private void UpdateSpriteSize() 
    {   
        spriteDisplay.size = baseSize + (Vector2.right * textDisplay.renderedWidth);
    }

    public void Hide()
    {
        spriteDisplay.enabled = false;    
        textDisplay.color = Color.clear;
        textDisplay.text = string.Empty;
    }
}
