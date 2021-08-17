using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable
{
    public override void Interaction(PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        gameObject.SetActive(false);
    }

    protected override void HighlightState(bool value) 
    {
        base.HighlightState(value);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = value ? Color.red : Color.white;
    }
}
