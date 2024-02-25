using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;
    [SerializeField] protected SpeechBubble speechBubble;
    [SerializeField] protected DialogueBoxStyle customDialogueStyle;
    [SerializeField] protected AK.Wwise.Event talkSoundAKEvent;

    public UnityAction OnInteraction;
    public UnityAction <int, NPCData> OnPreviousIndexReached;
    public UnityAction OnDialogueEnd;
    public UnityAction AfterDialogueEnd;

    protected PlayerInteractor interactor;

    public PlayerInteractor PlayerInteractor { get { return interactor; } }

    public override void Interaction (PlayerInteractor interactor) 
    {
        //Debug.Log($"Interaction - name: {name}");
        if (!interactable)
            return;

        base.Interaction (interactor);
        this.interactor = interactor;

        if (data)
        {
            if (OnInteraction != null)
                OnInteraction.Invoke();

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
            
            dialogSystem?.SetDialogue(this, npcName, dialogueGroup.tags, OnDialogueEnd, AfterDialogueEnd, customDialogueStyle, talkSoundAKEvent);

            if (interactor)
            {
                PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
                platformerCharacter?.KillInputs();
                platformerCharacter?.LookAtTarget(transform);
            }

            DialogueIndexLogic();
        }
    }

    protected virtual void DialogueIndexLogic() 
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
        if (!interactable)
            return;

        if (!speechBubble)
            return;
        
        if (value)
            speechBubble.Show();
        else
            speechBubble.Hide();
    }

    public override void IconState (bool value) 
    {
        if (!speechBubble)
            return;

        speechBubble.SetAnimationState(value);
    }

    public int GetDialogueIndex()
    {
        if (data)
            return data.Index;

        return -1;
    }
}
