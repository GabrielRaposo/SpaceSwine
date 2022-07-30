using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;
    [SerializeField] GameObject speechBalloonObject;
    [SerializeField] DialogueBoxStyle customDialogueStyle;

    public UnityAction <int, NPCData> OnPreviousIndexReached;
    public UnityAction OnDialogueEnd;

    private void Awake() 
    {
        HighlightState(false);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            DialogueSystem dialogSystem = DialogueSystem.Instance;
            DialogueGroup dialogueGroup = data.GetAtIndex();

            dialogSystem?.SetDialogue(this, data.npcName, dialogueGroup.tags, OnDialogueEnd, customDialogueStyle);

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
        if (speechBalloonObject)
            speechBalloonObject.SetActive(value);
    }

    public override void IconState (bool value) 
    {
        //if (speechBalloonObject)
        //    speechBalloonObject.SetActive(value);
    }

    public int GetDialogueIndex()
    {
        if (data)
            return data.Index;

        return -1;
    }
}
