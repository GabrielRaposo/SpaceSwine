using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStoryEventOnStart : StoryEventDependent
{
    [SerializeField] StoryEventScriptableObject storyEvent;

    void Start()
    {
        if (!storyEvent)
            return;

        CallDependentAction
        (
            action: () => 
            {
                if (StoryEventsManager.IsComplete(storyEvent))
                    return;

                StoryEventsManager.ChangeProgress (storyEvent, +99);
            }
        );
    }


}
