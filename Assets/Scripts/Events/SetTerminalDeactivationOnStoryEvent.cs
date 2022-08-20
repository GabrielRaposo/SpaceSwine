using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTerminalDeactivationOnStoryEvent : MonoBehaviour
{
    [SerializeField] StoryEventScriptableObject storyEvent;

    void OnEnable()
    {
        if (!storyEvent)
            return;

        storyEvent.OnStateChange += OnStateChange;

        //OnStateChange (storyEvent.state);
    }

    private void OnStateChange (bool state)
    {
        Interactable interactable = GetComponentInChildren<Interactable>();
        if (!interactable)
            return;

        interactable.autoActivate = false;
        interactable.SetInteraction(false);
    }

    private void OnDisable() 
    {
        if (!storyEvent)
            return;

        storyEvent.OnStateChange -= OnStateChange;
    }
}
