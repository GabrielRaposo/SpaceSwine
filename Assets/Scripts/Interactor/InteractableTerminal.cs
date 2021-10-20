using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTerminal : Interactable
{
    ITerminalEvent terminalEvent;

    private void Start()
    {
        terminalEvent = GetComponentInParent<ITerminalEvent>();
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (terminalEvent == null)
            return;

        terminalEvent.Activate(this);
    }

    protected override void HighlightState (bool value) 
    {
    }

    public override void IconState (bool value) 
    {
    }
}
