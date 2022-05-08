﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Minigame;

public enum ShipAction
{
    Talk,
    Leave,
    Chill,
    GGS,
    Customize
}

public class InteractableShipComponent : Interactable
{
    [SerializeField] ShipAction shipAction;
    [SerializeField] SpriteSwapper iconSwapper;
    [SerializeField] ShipInteractionBalloon interactionBalloon;
    [SerializeField] Color iconColorOff;
    [SerializeField] Color iconColorOn;

    Sequence highlightSequence;

    private void Awake() 
    {
        Collider2D _coll2D = GetComponent<Collider2D>();
        _coll2D.enabled = false;

        iconSwapper.SetSpriteState(0);
        interactionBalloon.SetInstantState(false);

        interactionBalloon.SetTextDisplay(shipAction);

        _coll2D.enabled = true;
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        switch (shipAction)
        {
            case ShipAction.GGS:

                if (GGSConsole.Instance)
                    GGSConsole.Instance.ToggleConsoleState();
                break;


            case ShipAction.Leave:

                if (ShipShuttleSystem.Instance)
                    ShipShuttleSystem.Instance.ExitOnShuttle();
                break;
        }
    }

    protected override void HighlightState (bool value) 
    {
        if (highlightSequence != null)
            highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        
        if (value)
            highlightSequence.Append(iconSwapper.transform.DOScale(2.0f, .1f));
        else
            highlightSequence.Append(iconSwapper.transform.DOScale(1.0f, .1f));
        
        highlightSequence.OnComplete( 
            () =>
            {
                iconSwapper.SetSpriteState(value ? 1 : 0);
                iconSwapper.GetComponent<SpriteRenderer>().color = value ? iconColorOn : iconColorOff;
            }
        ); 

        interactionBalloon.SetHighlight(value);
    }
}