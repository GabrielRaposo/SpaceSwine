﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitethornBuild;

public class InteractableDangerSceneEndCallerItem : Interactable
{
    [SerializeField] GameObject inputIcon;

    RoundsManager roundsManager;

    private void Start() 
    {
        roundsManager = GetComponentInParent<RoundsManager>();

        HighlightState(false);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (!roundsManager)
            return;

        if (interactor)
        {
            PlayerInput playerInput = interactor.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = interactor.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }

        // -- Chama caixa de diálogo do item sendo pego

        CallEnd (interactor);
    }

    private void CallEnd (PlayerInteractor interactor)
    {
        roundsManager.NextRoundLogic();
    }

    protected override void HighlightState (bool value) 
    {
        if (inputIcon)
            inputIcon.SetActive(value);
    }
}