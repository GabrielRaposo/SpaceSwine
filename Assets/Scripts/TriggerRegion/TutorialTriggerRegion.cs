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
        Debug.Log("hi: " + collision.name);
        if (!collision.CompareTag("Player"))
            return;    

        Debug.Log("a");
        if (showing)
            return;

        Debug.Log("b");
        if (tutorialTextBox)
        {
            Debug.Log("c");
            
            (bool valid, string text) localizedData = LocalizationManager.GetStoryText(textID);
            if (!localizedData.valid)
                return;

            Debug.Log("d");

            tutorialTextBox.ShowText(localizedData.text);
            showing = true;
        }
    }

    private void TriggerFuncition(Collider2D collision)
    {

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
