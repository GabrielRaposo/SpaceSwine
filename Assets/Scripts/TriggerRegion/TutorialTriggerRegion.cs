using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerRegion : MonoBehaviour
{
    [SerializeField] string textID;

    bool showing;
    TutorialTextBox tutorialTextBox;

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
            (bool valid, string text) localizedData = LocalizationManager.GetStoryText(textID);
            if (!localizedData.valid)
                return;

            tutorialTextBox.ShowText(localizedData.text);
            showing = true;
        }
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
