using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CustomRotateBursts))]
public class TerminalCallCustomRotate : TerminalCustomCaller, ITerminalEvent
{
    CustomRotateBursts customRotate;

    int calls;

    PlayerInteractor interactor;
    List<Interactable> interactableChildren; 

    private void Awake()
    {
        customRotate = GetComponent<CustomRotateBursts>();    
    }

    void Start()
    {
        interactableChildren = new List<Interactable>();
        GetComponentsInChildren(interactableChildren);

        customRotate.OnStepReached += OnStepReached;
        calls = 0;

        Round round = GetComponentInParent<Round>();
        if (round) round.OnReset += OnReset;
    }

    public void Activate (InteractableTerminal terminal, PlayerInteractor interactor) 
    {    
        this.interactor = interactor;
        BeforeSequence(interactor);

        calls++;
        customRotate.ResumeMovement();

        SetInteractionAction?.Invoke(false);
    }

    private void BeforeSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(false);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = player.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }
    }

    private void AfterSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(true);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }
    }

    private void OnStepReached()
    {
        if (interactor)
            AfterSequence(interactor);

        calls--;
        customRotate.PauseMovement();

        if (calls > 0)
            customRotate.ResumeMovement();
        else
            SetInteractionAction?.Invoke(true);
    }

    private void OnReset()
    {
        SetInteractionAction?.Invoke(true);
        calls = 0;
    }
}
