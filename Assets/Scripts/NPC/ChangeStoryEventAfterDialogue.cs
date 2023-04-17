using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class ChangeStoryEventAfterDialogue : MonoBehaviour
{
    [SerializeField] StoryEventScriptableObject storyEvent;
    [SerializeField] int progressValue = 1;
    [SerializeField] int dialogueGroupIndex = -1;

    void Start()
    {
        if (storyEvent == null)
            return;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            if (dialogueGroupIndex > -1 && dialogueGroupIndex != interactableNPC.GetDialogueIndex())
                return;    

            if (progressValue > 0)
                StoryEventsManager.ChangeProgress(storyEvent, progressValue);
            else
                StoryEventsManager.ClearProgress(storyEvent);

            SaveManager.SaveStoryEvent(storyEvent);
        };
    }
}
