using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogueBox dialogBox;

    public static bool OnDialogue;
    public static bool BlockInputs;
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
        UnityAction AfterDialogueEnd,
        DialogueBoxStyle customDialogueStyle,
        AK.Wwise.Event talkSoundAKEvent,
        UnityAction OnDialogueCancel = null
    )
    {
        if (!dialogBox)
            return;

        BlockInputs = false;
        
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

        dialogBox.SetDialogueData(interactable, speakerName, translatedDialogues, OnDialogueEnd, AfterDialogueEnd, customDialogueStyle, talkSoundAKEvent, OnDialogueCancel);
    }

    public void CancelDialogue()
    {
        if (!dialogBox)
            return;

        dialogBox.CancelDialogue();
    }

    private void OnDisable() 
    {
        OnDialogue = false;    
    }
}
