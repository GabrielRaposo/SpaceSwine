using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalLock : Lock
{
    public List<GameObject> terminalEventObjects;
    private List<ITerminalEvent> terminalEvent;

    protected override void Start()
    {
        base.Start();
        
        terminalEvent = new List<ITerminalEvent>();

        for (int i = 0; i < terminalEventObjects.Count; i++)
        {
            var te = terminalEventObjects[i].GetComponent<ITerminalEvent>();
            
            if (te != null)
                terminalEvent.Add(te);
        }
            
    }

    public override bool Collect (Collectable collectable)
    {
        base.Collect(collectable);
        
        if (terminalEvent == null) return true;

        for (int i = 0; i < terminalEvent.Count; i++)
        {
            terminalEvent[i].Activate(null,null);
        }

        return true;
    }

    public override void AnimationEndTrigger()
    {
        
    }
}
