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

    public void SetDialogue (Interactable interactable, string speakerName, List<string> dialog)
    {
        if (!dialogBox)
            return;

        dialogBox.SetDialogueData(interactable, speakerName, dialog);
    }

    private void OnDisable() 
    {
        OnDialogue = false;    
    }
}
