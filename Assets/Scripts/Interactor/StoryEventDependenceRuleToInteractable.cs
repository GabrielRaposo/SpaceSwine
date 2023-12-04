using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(InteractableShipComponent))]
public class StoryEventDependenceRuleToInteractable : StoryEventDependent
{
    [SerializeField] public List<StoryEventScriptableObject> storyEvents;

    InteractableShipComponent interactableShipComponent;

    private void Awake()
    {
        interactableShipComponent = GetComponent<InteractableShipComponent>();
    }

    private void Start()
    {
        if (storyEvents == null || storyEvents.Count < 1)
            return;

        CallDependentAction ( SetInteractableState, extraFrames: 1 );
    }

    private void SetInteractableState() 
    {
        bool disable = true;
                
        foreach (var key in storyEvents)
        {
            if (StoryEventsManager.IsComplete(key))
            {
                disable = false;
                break;
            }
        } 

        Debug.Log("Disable? " + disable);
        interactableShipComponent.DisableInteraction = disable;
    }
}
