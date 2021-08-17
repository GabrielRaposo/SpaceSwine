using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    [SerializeField] NPCData data;

    public override void Interaction(PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            DialogSystem dialogSystem = DialogSystem.Instance;
            if (dialogSystem)
                dialogSystem.SetDialog(data.npcName, data.dialog);
        }
        else gameObject.SetActive(false);
    }

    protected override void HighlightState(bool value) 
    {
        base.HighlightState(value);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = value ? Color.red : Color.white;
    }
}
