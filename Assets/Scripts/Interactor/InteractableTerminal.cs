using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTerminal : Interactable
{
    [SerializeField] bool active;
    [SerializeField] Animator terminalAnimator;
    [SerializeField] GameObject lightComponent;
    [SerializeField] GameObject inputIcon;

    public GameObject terminalEventObject;
    ITerminalEvent terminalEvent;

    private void Start()
    {
        if (terminalEventObject == null)
            terminalEvent = GetComponentInParent<ITerminalEvent>();
        else
            terminalEvent = terminalEventObject.GetComponent<ITerminalEvent>();
        
        CapsuleCollider2D capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        PlanetBlock planetBlock = GetComponent<PlanetBlock>();

        if (planetBlock && capsuleCollider2D)
        {
            capsuleCollider2D.offset = new Vector2(capsuleCollider2D.offset.x, planetBlock.GetSize() - .5f);
        }

        if (terminalAnimator == null)
            terminalAnimator = GetComponentInChildren<Animator>();

        UpdateAnimationState();
        HighlightState(false);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        if (!active || !interactable)
            return;

        base.Interaction(interactor);

        if (terminalEvent == null)
            return;

        terminalEvent.Activate(this, interactor);
    }

    public override void SetInteraction(bool value) 
    {
        base.SetInteraction(value);

        //active = value;
        //UpdateAnimationState();
    }

    protected override void HighlightState (bool value) 
    {
        inputIcon?.SetActive(value && active);
    }

    public override void IconState (bool value) 
    {

    }

    private void UpdateAnimationState()
    {
        if (!terminalAnimator)
            return;

        string animationName = active ? "PCTerminalOn1" : "PCTerminalOff";
        terminalAnimator.Play(animationName);
        lightComponent?.SetActive(active);
    }
}
