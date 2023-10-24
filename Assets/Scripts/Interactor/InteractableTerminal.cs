using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTerminal : Interactable
{
    private Round _round;
    
    [SerializeField] bool active;
    [SerializeField] Animator terminalAnimator;
    [SerializeField] GameObject lightComponent;
    [SerializeField] GameObject inputIcon;
    [SerializeField] AK.Wwise.Event terminalAKEvent;

    public bool isSingleUse;

    public GameObject terminalEventObject;
    ITerminalEvent terminalEvent;

    private void Start()
    {
        if (terminalEventObject == null)
            terminalEvent = GetComponentInParent<ITerminalEvent>();
        else
            terminalEvent = terminalEventObject.GetComponent<ITerminalEvent>();

        SetCustomCallerInteraction();
        
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
        
        _round = GetComponentInParent<Round>();
        _round.OnReset += OnReset;
    }

    private void OnReset()
    {
        active = true;
        UpdateAnimationState();
    }

    private void SetCustomCallerInteraction()
    {
        if (!terminalEventObject)
            return;

        TerminalCustomCaller customCaller = terminalEventObject.GetComponent<TerminalCustomCaller>();
        if (!customCaller)
            return;

        customCaller.SetInteractionAction += (value) => 
        {
            SetInteraction(value);
        };
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        if (!active || !interactable)
            return;

        if (isSingleUse)
        {
            active = false;
            UpdateAnimationState();
        }

        base.Interaction(interactor);
        
        if (terminalAnimator)
            terminalAnimator.SetTrigger("interact");
            

        if (terminalEvent == null)
            return;

        terminalAKEvent?.Post(gameObject);
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

        //string animationName = active ? "PCTerminalOn1" : "PCTerminalOff";
        //terminalAnimator.Play(animationName);
        
        terminalAnimator.SetBool("isOn", active);
        
        lightComponent?.SetActive(active);
        
        if(!active)
            inputIcon?.SetActive(false);
    }
}
