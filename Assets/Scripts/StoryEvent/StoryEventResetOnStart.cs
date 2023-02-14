﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEventResetOnStart : MonoBehaviour
{
    [SerializeField] List<StoryEventScriptableObject> storyEvents;

    void Start()
    {
        SetTerminalActivationOnTheFirstTime.FirstTime = true;
        SetShipDialogueOnEvent.World1ShipDialogueSet = false;
        SetShipDialogueOnEvent.World2ShipDialogueSet = false;
        SetShipDialogueOnEvent.World3ShipDialogueSet = false;
        ShipDialogueManager.StartDialogueIndex = -1;

        if (storyEvents == null)
            return;

        foreach(StoryEventScriptableObject storyEvent in storyEvents)
        {
            storyEvent.SetState(false);
        }
    }
}