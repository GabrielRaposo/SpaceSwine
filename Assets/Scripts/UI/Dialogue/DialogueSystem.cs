using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogueBox dialogBox;

    public static bool OnDialogue;
    public static DialogueSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetDialogue 
    (
        Interactable interactable, 
        string speakerName, 
        List<string> dialogueTags, 
        UnityAction OnDialogueEnd,
        DialogueBoxStyle customDialogueStyle,
        AK.Wwise.Event talkSoundAKEvent
    )
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

        dialogBox.SetDialogueData(interactable, speakerName, translatedDialogues, OnDialogueEnd, customDialogueStyle, talkSoundAKEvent);
    }

    private void OnDisable() 
    {
        OnDialogue = false;    
    }
}
