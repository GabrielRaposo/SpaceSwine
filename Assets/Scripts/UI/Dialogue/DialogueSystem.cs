using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogueBox dialogBox;

    public static bool OnDialogue;
    public static DialogueSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetDialogue (Interactable interactable, string speakerName, List<string> dialogueTags, DialogueBoxStyle customDialogueStyle)
    {
        if (!dialogBox)
            return;
        
        List<string> translatedDialogues = new List<string>();
        if (dialogueTags != null)
        {
            foreach (string tag in dialogueTags)
            {
                (bool valid, string text) localizedData = LocalizationManager.GetStoryText(tag);
                
                if (!localizedData.valid)
                    continue;

                translatedDialogues.Add(localizedData.text);
            }
        }

        dialogBox.SetDialogueData(interactable, speakerName, translatedDialogues, customDialogueStyle);
    }

    private void OnDisable() 
    {
        OnDialogue = false;    
    }
}
