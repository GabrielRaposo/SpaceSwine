using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialTextBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    CanvasGroup canvasGroup;
    public static TutorialTextBox Instance;

    private void Awake() 
    {
        Instance = this;    
        canvasGroup = GetComponent<CanvasGroup>();

        HideText();
    }

    public void ShowText (string text)
    {
        canvasGroup.alpha = 1;
        if (textDisplay)
            textDisplay.text = text;
    }

    public void HideText ()
    {
        canvasGroup.alpha = 0;
        if (textDisplay)
            textDisplay.text = string.Empty;
    }
}
