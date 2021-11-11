using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;
    [SerializeField] SpriteSwapper ballonSpriteSwapper;
    [SerializeField] DialogueBoxStyle customDialogueStyle;

    public UnityAction <int, NPCData> OnPreviousIndexReached;

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            DialogueSystem dialogSystem = DialogueSystem.Instance;
            DialogueGroup dialogueGroup = data.GetAtIndex();

            dialogSystem?.SetDialogue(this, data.npcName, dialogueGroup.tags, customDialogueStyle);

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
        Debug.Log("previousIndex: " + previousIndex);
        data.MoveIndex(1);

        OnPreviousIndexReached?.Invoke(previousIndex, data);
    }

    protected override void HighlightState (bool value) 
    {
        if (ballonSpriteSwapper)
            ballonSpriteSwapper.SetSpriteState ( value ? 1 : 0 );
    }

    public override void IconState (bool value) 
    {
        if (ballonSpriteSwapper)
            ballonSpriteSwapper.SetVisibility(value);
    }
}
