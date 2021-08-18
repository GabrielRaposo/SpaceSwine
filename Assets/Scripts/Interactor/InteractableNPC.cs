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
            DialogSystem dialogSystem = DialogSystem.Instance;
            dialogSystem?.SetDialog(data.npcName, data.dialog);

            if (interactor)
            {
                PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
                platformerCharacter?.KillInputs();
            }
        }
    }

    protected override void HighlightState(bool value) 
    {
        base.HighlightState(value);

        //SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //spriteRenderer.color = value ? Color.blue : Color.white;

        if (ballonSpriteSwapper)
            ballonSpriteSwapper.SetSpriteState ( value ? 1 : 0 );
    }
}
