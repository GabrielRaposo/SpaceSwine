using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerRegion : MonoBehaviour
{
    [SerializeField] string textID;

    bool showing;
    TutorialTextBox tutorialTextBox;

    private void OnEnable() 
    {
        InputTagController.OnInputTypeChanged += UpdateText;    
    }

    private void OnDisable() 
    {
        InputTagController.OnInputTypeChanged -= UpdateText;
    }

    void Start()
    {
        tutorialTextBox = TutorialTextBox.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (showing)
            return;

        if (tutorialTextBox)
        {
            showing = true;
            UpdateText(); 
        }
    }

    private void UpdateText()
    {
        if (!showing)
            return;

        (bool valid, string text) localizedData = LocalizationManager.GetStoryText(textID);

        if (!localizedData.valid)
            return;

        string text = ParseInputTag.ParsedOutput (localizedData.text, ":");

        tutorialTextBox.ShowText(text);
    }

    private void OnTriggerExit2D (Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;

        if (tutorialTextBox)
        {
            tutorialTextBox.HideText();
            showing = false;
        } 
    }
}
