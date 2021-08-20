using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;
    [SerializeField] SpriteSwapper ballonSpriteSwapper;

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            DialogueSystem dialogSystem = DialogueSystem.Instance;
            dialogSystem?.SetDialogue(this, data.npcName, data.dialog);

            if (interactor)
            {
                PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
                platformerCharacter?.KillInputs();
                platformerCharacter?.LookAtTarget(transform);
            }
        }
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
