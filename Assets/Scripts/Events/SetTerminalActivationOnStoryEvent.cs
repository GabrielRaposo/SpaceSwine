using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTerminalActivationOnStoryEvent : MonoBehaviour
{
    [SerializeField] bool targetState;
    [SerializeField] StoryEventScriptableObject storyEvent;

    void OnEnable()
    {
        SaveManager.IsSaveReady();

        if (!storyEvent)
            return;

        //storyEvent.OnStateChange += OnStateChange;
        //OnStateChange (storyEvent.StartingState);

        StoryEventsManager.AddListener(storyEvent, OnStateChange);
        OnStateChange( StoryEventsManager.IsComplete(storyEvent) );
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
        if (!storyEvent)
            return;

        //storyEvent.OnStateChange -= OnStateChange;

        StoryEventsManager.RemoveListener(storyEvent, OnStateChange);
    }
}
