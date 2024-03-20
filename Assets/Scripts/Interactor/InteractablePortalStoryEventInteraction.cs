using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractablePortal))]
public class InteractablePortalStoryEventInteraction : StoryEventDependent
{
    const float SILENCE_THRESHOLD = 1.0f;

    [SerializeField] StoryEventScriptableObject storyEventData;
    [SerializeField] Door door;

    InteractablePortal interactablePortal;

    float t;

    void Start()
    {
        interactablePortal = GetComponent<InteractablePortal>();

        CallDependentAction ( Setup, extraFrames: 3 );

        //Debug.Log("TO-FIX: Running on Update!");
    }

    private void Setup()
    {
        if (storyEventData == null || door == null)
            return;

        this.Wait 
        (
            frames: 3,
            action: () => 
            {
                bool IsComplete = StoryEventsManager.IsComplete(storyEventData);

                interactablePortal.SetInteraction(IsComplete);
                door.SetOpenState(IsComplete, instant: true);
                //Debug.Log("Setup!: " + IsComplete);
            }
        );
    }

    private void Update() 
    {
        if (storyEventData == null || door == null)
            return;

        if (!StoryEventsManager.Initiated)
            return;

        bool IsComplete = StoryEventsManager.IsComplete(storyEventData);

        if (t < SILENCE_THRESHOLD)
            t += Time.deltaTime;

        if (IsComplete)
        {
            interactablePortal.SetInteraction(IsComplete);
            door.ExternalOpenDoorCall(silent: t < SILENCE_THRESHOLD);

            enabled = false;
        }
    }
}
