using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTerminalActivationOnStoryEvent : StoryEventDependent
{
    [SerializeField] bool targetState;
    [SerializeField] StoryEventScriptableObject storyEvent;

    void OnEnable()
    {
        CallDependentAction
        (
            action: () => 
            {
                if (!storyEvent)
                    return;

                StoryEventsManager.AddListener(storyEvent, OnStateChange);
                OnStateChange( StoryEventsManager.IsComplete(storyEvent) );
            }
        );
    }

    private void OnStateChange (bool state)
    {
        if (state != targetState)
            return;

        Interactable interactable = GetComponentInChildren<Interactable>();
        if (!interactable)
            return;

        interactable.autoActivate = false;
        interactable.SetInteraction(state);
    }

    private void OnDisable() 
    {
        CallDependentAction
        (
            action: () => 
            {
                if (!storyEvent)
                    return;

                StoryEventsManager.RemoveListener(storyEvent, OnStateChange);
            },
            extraFrames: 1
        );
    }
}
