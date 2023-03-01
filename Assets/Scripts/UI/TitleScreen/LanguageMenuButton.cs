using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageMenuButton : MonoBehaviour
{
    public GameLocalizationCode LocalizationCode;
    [SerializeField] GameObject highlightDisplay;

    bool highlighted;

    private void Awake() 
    {
        Deselect();
    }

    public void Select ()
    {
        if (highlightDisplay)
            highlightDisplay.SetActive(true);
            
        highlighted = true;
    }

    public void Deselect ()
    {
        if (highlightDisplay)
            highlightDisplay.SetActive(false);
            
        highlighted = false;
    }
}
