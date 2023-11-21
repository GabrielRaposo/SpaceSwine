﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Minigame;


public enum ShipAction
{
    Talk,
    Leave,
    Chill,
    GGS,
    Customize,
    Navigate,
    MakeABeat
}

public class InteractableShipComponent : Interactable
{
    [SerializeField] bool disableInteraction;
    [SerializeField] List<ShipAction> shipActions;
    
    [Header("References")]
    [SerializeField] ShipInteractionBalloon interactionBalloon;
    [SerializeField] SpriteRenderer arrowUp;
    [SerializeField] SpriteRenderer arrowDown;
    [SerializeField] private ShipDialogueManager shipDialogueManager;

    [Header("Icon")]
    [SerializeField] SpriteSwapper iconSwapper;
    [SerializeField] Color iconColorOff;
    [SerializeField] Color iconColorOn;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event cursorMoveAKEvent;
    [SerializeField] AK.Wwise.Event cursorSelectAKEvent;

    int index;
    PlayerInputActions inputActions;
    
    bool highlighted;
    Sequence highlightSequence;

    private ShipAction CurrentShipAction { get { return shipActions[index % shipActions.Count]; } }

    private void Awake() 
    {
        Collider2D _coll2D = GetComponent<Collider2D>();
        _coll2D.enabled = false;

        iconSwapper.SetSpriteState(0);
        
        index = 0;
        
        interactionBalloon.SetInstantState(false);
        UpdateTextDisplay();

        _coll2D.enabled = true;

        if (disableInteraction)
            SetInteraction(false);
    }


    private void UpdateTextDisplay()
    {
        interactionBalloon.SetTextDisplay(CurrentShipAction);
        
        if (arrowUp)
            arrowUp.enabled = index > 0;
        
        if (arrowDown)
            arrowDown.enabled = index < shipActions.Count - 1;
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        if (!interactable)
        {
            DebugDisplay.Call("Cannot interact.");
            return;
        }

        base.Interaction(interactor);

        switch (CurrentShipAction)
        {
            case ShipAction.Talk:
                
                shipDialogueManager.CallDialogue();
                break;
            
            case ShipAction.GGS:

                if (GGSConsole.Instance)
                    GGSConsole.Instance.ToggleConsoleState();
                break;

            case ShipAction.Leave:

                if (ShipInitializerSystem.Instance)
                    ShipInitializerSystem.Instance.ExitOnShuttle();
                break;

            case ShipAction.Chill:

                if (RestLoopManager.Instance)
                    RestLoopManager.Instance.TurnOn();
                break;
            
            case ShipAction.Navigate:

                if (NavigationConsole.Instance)
                    NavigationConsole.Instance.ToggleConsoleState();
                break;

            case ShipAction.MakeABeat:
                
                if (PlayerCharacter.Instance)
                    PlayerCharacter.Instance.DisableAllInteractions();
                if (SoundtrackManager.Instance)
                    SoundtrackManager.Instance.FadeOutMusic(1.0f);
                GameManager.GoToScene(BuildIndex.MakeABeat);
                break;
        }

        if (cursorSelectAKEvent != null)
            cursorSelectAKEvent.Post(gameObject);
    }

    protected override void HighlightState (bool value) 
    {
        if (GameManager.OnDialogue)
            value = false;

        if (highlighted == value)
            return;
        highlighted = value;

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

        if (value)
            EnableInputs();
        else
            DisableInputs();
    }

    private void EnableInputs()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Movement.performed += (ctx) => 
        {
            if (PauseSystem.OnPause || GameManager.BlockCharacterInput)
                return;

            Vector2 input = ctx.ReadValue<Vector2>();
            float threshold = .9f;
            if (Mathf.Abs (input.normalized.y) > threshold)
                MoveIndex (input.y > 0 ? -1 : 1);
        }; 
        inputActions.Player.Movement.Enable();

        inputActions.Enable();
    }

    private void MoveIndex(int direction)
    {
        int previousIndex = index;
        index += direction;

        if (index < 0)
            index = 0;

        if (index > shipActions.Count - 1)
            index = shipActions.Count - 1;

        if (cursorMoveAKEvent != null && previousIndex != index)
            cursorMoveAKEvent.Post(gameObject);

        UpdateTextDisplay();
    }

    private void DisableInputs()
    {
        if (inputActions == null)
            return;

        inputActions.Disable();
        inputActions.Player.Movement.Disable();
    }

    public override void SetInteraction(bool value) 
    {
        base.SetInteraction(value);

        interactionBalloon.SetInteractableState(value);
    }

    private void OnEnable()
    {
        LocalizationManager.AddToLanguageChangeActionList(UpdateTextDisplay);
    }

    private void OnDisable()
    {
        LocalizationManager.RemoveFromLanguageChangeActionList(UpdateTextDisplay);
        DisableInputs();
    }
}
