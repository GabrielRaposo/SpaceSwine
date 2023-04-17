using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class ChangeStoryEventAfterDialogue : MonoBehaviour
{
    [SerializeField] StoryEventScriptableObject storyEvent;
    [SerializeField] bool targetValue = true;
    [SerializeField] int dialogueGroupIndex = -1;

    void Start()
    {
        if (storyEvent == null)
            return;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            //Debug.Log("interactableNPC.GetDialogueIndex(): " + interactableNPC.GetDialogueIndex());

            if (dialogueGroupIndex > -1 && dialogueGroupIndex != interactableNPC.GetDialogueIndex())
            {
                return;    
            }

            storyEvent.State = targetValue;
            SaveManager.SaveStoryEvent(storyEvent);

            //Debug.Log("Dialogue ended.");
        };
    }
}
