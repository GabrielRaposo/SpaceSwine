using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalLock : Lock
{
    public GameObject terminalEventObject;
    ITerminalEvent terminalEvent;

    private void Start()
    {
        if (terminalEventObject == null)
            terminalEvent = GetComponentInParent<ITerminalEvent>();
        else
            terminalEvent = terminalEventObject.GetComponent<ITerminalEvent>();
    }

    public override void Collect(Collectable collectable)
    {
        base.Collect(collectable);
        
        if(terminalEvent == null) return;
        
        terminalEvent.Activate(null, null);
        
    }
}
