﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetShipDialogueOnEvent : StoryEventDependent
{
    [SerializeField] bool setOnStart;

    [Header("Story Event (Opcional)")]
    [SerializeField] StoryEventScriptableObject storyEvent;
    [SerializeField] UnityEvent OnTrueStoryEvent; // -- Deve chamar um SetWorldState() pelo editor

    #region TEMP
    public static bool World1ShipDialogueSet;
    public static bool World2ShipDialogueSet;
    public static bool World3ShipDialogueSet;

    public static bool AllDialoguesSet
    {
        get 
        {
            //return true;
            return World1ShipDialogueSet && World2ShipDialogueSet && World3ShipDialogueSet;
        }
    }
    #endregion

    void OnEnable()
    {
        CallDependentAction
        (
            action: () =>
            {
                if (storyEvent == null)
                    return;

                StoryEventsManager.AddListener(storyEvent, SubscribeToStoryEvent);
                SubscribeToStoryEvent(StoryEventsManager.IsComplete (storyEvent) );
            }
        );
    }

    private void Start() 
    {
        if (setOnStart)
        {
            SubscribeToStoryEvent(true);
        }
    }

    private void SubscribeToStoryEvent (bool value)
    {
        //Debug.Log("value: " + value);
        if (!value)
            return;

        OnTrueStoryEvent.Invoke();
    }

    public void SetWorldState (int state)
    {
        if (ShipDialogueManager.StartDialogueIndex > -1)
            return;

        switch(state)
        {
            default:
                break;

            case 1:
                if (World1ShipDialogueSet)
                    return;
                World1ShipDialogueSet = true;
                Debug.Log("World1ShipDialogueSet");
                break;

            case 2:
                if (World2ShipDialogueSet)
                    return;
                World2ShipDialogueSet = true;
                Debug.Log("World2ShipDialogueSet");
                break;

            case 3:
                if (World3ShipDialogueSet)
                    return;
                World3ShipDialogueSet = true;
                Debug.Log("World3ShipDialogueSet");
                break;
        }

        Debug.Log("Set event: " + state);
        ShipDialogueManager.StartDialogueIndex = state - 1;
    }

    private void OnDisable() 
    {
        CallDependentAction
        (
            action: () =>
            {
                if (!storyEvent)
                    return;

                StoryEventsManager.RemoveListener(storyEvent, SubscribeToStoryEvent);
            },
            extraFrames: 1
        );

    }
}
