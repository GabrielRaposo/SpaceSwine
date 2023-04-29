using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNotificationOnStoryEvent : StoryEventDependent
{
    [SerializeField] StoryEventScriptableObject storyEvent;
    [SerializeField] string notificationID;

    private void OnEnable() 
    {
        CallDependentAction
        (
            () =>
            {
                if (storyEvent == null)
                    return;
 
                StoryEventsManager.AddListener(storyEvent, SetNotification);
            }
        );
    }

    private void Start() 
    {
        CallDependentAction
        (
            () =>
            {
                if (storyEvent == null)
                    return;

                SetNotification(StoryEventsManager.IsComplete(storyEvent));
            }
        );
    }

    private void SetNotification (bool state)
    {
        Debug.Log("state: " + state);

        if (!state)
            return;

        if (string.IsNullOrEmpty(notificationID))
            return;

        DebugDisplay.Call ($"Create notificationID: {notificationID}");
        UINotificationManager.Create(notificationID);
    }

    private void OnDisable() 
    {
        CallDependentAction
        (
            () =>
            {
                if (storyEvent == null)
                    return;
 
                StoryEventsManager.RemoveListener(storyEvent, SetNotification);
            }, 
            extraFrames: 1
        );
    }
}
