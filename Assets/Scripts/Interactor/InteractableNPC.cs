﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;
    [SerializeField] SpeechBubble speechBubble;
    [SerializeField] DialogueBoxStyle customDialogueStyle;
    [SerializeField] AK.Wwise.Event talkSoundAKEvent;

    public UnityAction <int, NPCData> OnPreviousIndexReached;
    public UnityAction OnDialogueEnd;

    public override void Interaction (PlayerInteractor interactor) 
    {
        //Debug.Log($"Interaction - name: {name}");

        base.Interaction(interactor);

        if (data)
        {
            //Debug.Log("data:  " + data);

            DialogueSystem dialogSystem = DialogueSystem.Instance;
            DialogueGroup dialogueGroup = data.GetAtIndex();

            //Debug.Log("data.npcNameCode: " + data.npcNameCode);
            string npcName = string.Empty;
            if (!string.IsNullOrEmpty (data.npcNameCode) )
            {
                (bool isValid, string text) nameData = LocalizationManager.GetStoryText(data.npcNameCode);
                if (nameData.isValid)
                    npcName = nameData.text;
            }
            
            dialogSystem?.SetDialogue(this, npcName, dialogueGroup.tags, OnDialogueEnd, customDialogueStyle, talkSoundAKEvent);

            if (interactor)
            {
                PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
                platformerCharacter?.KillInputs();
                platformerCharacter?.LookAtTarget(transform);
            }

            DialogueIndexLogic();
        }
    }

    private void DialogueIndexLogic() 
    {   
        if (!data)
            return;

        int previousIndex = data.Index;
        //Debug.Log("previousIndex: " + previousIndex);
        data.MoveIndex(1);

        OnPreviousIndexReached?.Invoke(previousIndex, data);
    }

    protected override void HighlightState (bool value) 
    {
        if (speechBubble)
        {
            if (value)
                speechBubble.Show();
            else
                speechBubble.Hide();
        }
    }

    public override void IconState (bool value) 
    {
        if (speechBubble)
        {
            speechBubble.SetAnimationState(value);
        }
    }

    public int GetDialogueIndex()
    {
        if (data)
            return data.Index;

        return -1;
    }
}
