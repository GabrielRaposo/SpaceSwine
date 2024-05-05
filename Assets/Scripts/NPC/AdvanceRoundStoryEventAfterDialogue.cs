using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(InteractableNPC))]
public class AdvanceRoundStoryEventAfterDialogue : MonoBehaviour
{
    void Start()
    {
        RoundsManager roundsManager = GetComponentInParent<RoundsManager>();
        if (!roundsManager || roundsManager.completionStoryEvent == null) 
            return;

        StoryEventScriptableObject storyEvent = roundsManager.completionStoryEvent;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            StoryEventsManager.ChangeProgress(storyEvent, 1);
            SaveManager.Save();
        };
    }
}
